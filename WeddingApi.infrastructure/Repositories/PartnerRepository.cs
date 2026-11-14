using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using WeddingApi.core.Common;
using WeddingApi.core.Entities;
using WeddingApi.core.Interfaces;
using WeddingApi.infrastructure.Data;

namespace WeddingApi.infrastructure.Repositories;

public class PartnerRepository : IPartnerRepository
{
    // من غير الحروف المتشابهة (O/0 و I/1) عشان الكود يتقري ويتكتب بسهولة.
    private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    private const int CodeLength = 8;

    private readonly WeddingDbContext _db;
    public PartnerRepository(WeddingDbContext db) => _db = db;

    private static string GenerateCode()
    {
        var chars = new char[CodeLength];
        for (var i = 0; i < chars.Length; i++)
            chars[i] = Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)];
        return new string(chars);
    }

    private static string Normalize(string code) =>
        (code ?? string.Empty).Trim().Replace("-", "").Replace(" ", "").ToUpperInvariant();

    public async Task<PartnerInvite?> GetActiveInviteAsync(int inviterClientId)
    {
        var now = DateTime.UtcNow;
        return await _db.PartnerInvites
            .Where(i => i.InviterClientId == inviterClientId && i.UsedAt == null && i.ExpiresAt > now)
            .OrderByDescending(i => i.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<PartnerInvite> CreateInviteAsync(int inviterClientId, TimeSpan lifetime)
    {
        var now = DateTime.UtcNow;

        var active = await _db.PartnerInvites
            .Where(i => i.InviterClientId == inviterClientId && i.UsedAt == null && i.ExpiresAt > now)
            .ToListAsync();
        foreach (var old in active)
            old.ExpiresAt = now;

        string code;
        do { code = GenerateCode(); }
        while (await _db.PartnerInvites.AnyAsync(i => i.Code == code));

        var invite = new PartnerInvite
        {
            InviterClientId = inviterClientId,
            Code = code,
            CreatedAt = now,
            ExpiresAt = now.Add(lifetime)
        };

        _db.PartnerInvites.Add(invite);
        await _db.SaveChangesAsync();
        return invite;
    }

    public async Task<PartnerLinkResult> AcceptAsync(string code, int acceptingClientId)
    {
        var normalized = Normalize(code);
        var now = DateTime.UtcNow;

        var invite = await _db.PartnerInvites
            .FirstOrDefaultAsync(i => i.Code == normalized && i.UsedAt == null && i.ExpiresAt > now);
        if (invite == null) return PartnerLinkResult.InvalidCode;

        if (invite.InviterClientId == acceptingClientId) return PartnerLinkResult.SelfInvite;

        var me = await _db.Clients.FirstOrDefaultAsync(c => c.Id == acceptingClientId);
        var inviter = await _db.Clients.FirstOrDefaultAsync(c => c.Id == invite.InviterClientId);
        if (me == null || inviter == null) return PartnerLinkResult.ClientNotFound;

        if (me.LinkedPartnerId > 0 || inviter.LinkedPartnerId > 0) return PartnerLinkResult.AlreadyLinked;

        if (string.Equals(me.Gender, inviter.Gender, StringComparison.OrdinalIgnoreCase))
            return PartnerLinkResult.SameGender;

        // الربط واستهلاك الكود في Save واحد: يا الكل يتم يا ولا حاجة.
        me.LinkedPartnerId = inviter.Id;
        inviter.LinkedPartnerId = me.Id;
        invite.UsedAt = now;
        invite.UsedByClientId = me.Id;

        await _db.SaveChangesAsync();
        return PartnerLinkResult.Success;
    }

    public async Task<bool> UnlinkAsync(int clientId)
    {
        var me = await _db.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
        if (me == null || me.LinkedPartnerId <= 0) return false;

        var partner = await _db.Clients.FirstOrDefaultAsync(c => c.Id == me.LinkedPartnerId);
        if (partner != null && partner.LinkedPartnerId == me.Id)
            partner.LinkedPartnerId = 0;

        me.LinkedPartnerId = 0;
        await _db.SaveChangesAsync();
        return true;
    }
}
