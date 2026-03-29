using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingApi.core.Entities;
using WeddingApi.infrastructure.Data;

namespace WeddingApi.web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly WeddingDbContext _db;
    private readonly Cloudinary _cloudinary;

    public MediaController(WeddingDbContext db, IConfiguration config)
    {
        _db = db;
        var account = new Account(
            config["Cloudinary:CloudName"],
            config["Cloudinary:ApiKey"],
            config["Cloudinary:ApiSecret"]
        );
        _cloudinary = new Cloudinary(account);
    }

    // GET: api/media/{serviceProviderId}
    [HttpGet("{serviceProviderId}")]
    public async Task<IActionResult> GetMedia(int serviceProviderId)
    {
        var media = await _db.ServiceProviderMedias
            .Where(m => m.ServiceProviderId == serviceProviderId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
        return Ok(media);
    }

    // POST: api/media/upload/{serviceProviderId}
    [HttpPost("upload/{serviceProviderId}")]
    [Authorize]
    public async Task<IActionResult> Upload(int serviceProviderId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("لا يوجد ملف");

        var isVideo = file.ContentType.StartsWith("video/");

        using var stream = file.OpenReadStream();

        string publicId;
        string url;

        if (isVideo)
        {
            var uploadParams = new VideoUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = $"wedding/providers/{serviceProviderId}"
            };
            var result = await _cloudinary.UploadAsync(uploadParams);
            if (result.Error != null) return BadRequest(result.Error.Message);
            publicId = result.PublicId;
            url = result.SecureUrl.ToString();
        }
        else
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = $"wedding/providers/{serviceProviderId}",
                Transformation = new Transformation().Quality("auto").FetchFormat("auto")
            };
            var result = await _cloudinary.UploadAsync(uploadParams);
            if (result.Error != null) return BadRequest(result.Error.Message);
            publicId = result.PublicId;
            url = result.SecureUrl.ToString();
        }

        var media = new ServiceProviderMedia
        {
            ServiceProviderId = serviceProviderId,
            Url = url,
            PublicId = publicId,
            MediaType = isVideo ? "video" : "image"
        };

        _db.ServiceProviderMedias.Add(media);
        await _db.SaveChangesAsync();

        return Ok(media);
    }

    // DELETE: api/media/{id}
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var media = await _db.ServiceProviderMedias.FindAsync(id);
        if (media == null) return NotFound();

        // حذف من Cloudinary
        var deleteParams = media.MediaType == "video"
            ? new DeletionParams(media.PublicId) { ResourceType = ResourceType.Video }
            : new DeletionParams(media.PublicId);

        await _cloudinary.DestroyAsync(deleteParams);

        _db.ServiceProviderMedias.Remove(media);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}