using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WeddingApi.core.Entities;
using WeddingApi.core.Interfaces;

namespace WeddingApi.web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly IUnitOfWorks _unitOfWork;
    private readonly Cloudinary _cloudinary;

    public MediaController(IUnitOfWorks unitOfWork, IConfiguration config)
    {
        _unitOfWork = unitOfWork;
        var account = new Account(
            config["Cloudinary:CloudName"],
            config["Cloudinary:ApiKey"],
            config["Cloudinary:ApiSecret"]
        );
        _cloudinary = new Cloudinary(account);
    }

    // true لو اليوزر الحالي Provider ومش صاحب الـ ServiceProviderId ده (يعني ممنوع).
    // الأدمن/المشرف ليهم استثناء دايمًا.
    private bool IsForbiddenForProvider(int serviceProviderId)
    {
        if (!User.IsInRole("Provider")) return false;

        var tokenUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return !int.TryParse(tokenUserId, out var myId) || myId != serviceProviderId;
    }

    // GET: api/media/{serviceProviderId}
    // مسموح للزوار (بدون تسجيل دخول) لأنها صور عرض عامة على صفحة مزود الخدمة.
    [HttpGet("{serviceProviderId}")]
    public async Task<IActionResult> GetMedia(int serviceProviderId)
    {
        var media = await _unitOfWork.Media.GetByProviderIdAsync(serviceProviderId);
        return Ok(media);
    }

    // POST: api/media/upload/{serviceProviderId}
    [HttpPost("upload/{serviceProviderId}")]
    [Authorize(Roles = "Provider,Admin,Supervisor")]
    public async Task<IActionResult> Upload(int serviceProviderId, IFormFile file)
    {
        if (IsForbiddenForProvider(serviceProviderId))
            return Forbid();

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

        var created = await _unitOfWork.Media.CreateAsync(media);

        return Ok(created);
    }

    // DELETE: api/media/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Provider,Admin,Supervisor")]
    public async Task<IActionResult> Delete(int id)
    {
        var media = await _unitOfWork.Media.GetByIdAsync(id);
        if (media == null) return NotFound();

        if (IsForbiddenForProvider(media.ServiceProviderId))
            return Forbid();

        // حذف من Cloudinary
        var deleteParams = media.MediaType == "video"
            ? new DeletionParams(media.PublicId) { ResourceType = ResourceType.Video }
            : new DeletionParams(media.PublicId);

        await _cloudinary.DestroyAsync(deleteParams);

        await _unitOfWork.Media.DeleteAsync(id);

        return NoContent();
    }
}