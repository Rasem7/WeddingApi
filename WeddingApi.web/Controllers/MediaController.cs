using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    // GET: api/media/{serviceProviderId}
    // مسموح للزوار (بدون تسجيل دخول) لأنها صور عرض عامة على صفحة مزود الخدمة.
    [HttpGet("{serviceProviderId}")]
    public async Task<IActionResult> GetMedia(int serviceProviderId)
    {
        var media = await _unitOfWork.Media.GetByProviderIdAsync(serviceProviderId);
        return Ok(media);
    }

    // POST: api/media/upload/{serviceProviderId}
    // TODO: لازم نتحقق إن serviceProviderId ده فعلًا ملك اليوزر صاحب التوكن
    // (أو إن اليوزر Admin/Supervisor) قبل ما نسمح بالرفع — حاليًا أي Provider
    // مسجل دخول يقدر يرفع صور لأي مزود خدمة تاني. هنعالجها لما نربط التوكن
    // بالـ ServiceProviderId في الـ claims.
    [HttpPost("upload/{serviceProviderId}")]
    [Authorize(Roles = "Provider,Admin,Supervisor")]
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

        var created = await _unitOfWork.Media.CreateAsync(media);

        return Ok(created);
    }

    // DELETE: api/media/{id}
    // TODO: نفس ملاحظة التحقق من الملكية المذكورة فوق الـ Upload.
    [HttpDelete("{id}")]
    [Authorize(Roles = "Provider,Admin,Supervisor")]
    public async Task<IActionResult> Delete(int id)
    {
        var media = await _unitOfWork.Media.GetByIdAsync(id);
        if (media == null) return NotFound();

        // حذف من Cloudinary
        var deleteParams = media.MediaType == "video"
            ? new DeletionParams(media.PublicId) { ResourceType = ResourceType.Video }
            : new DeletionParams(media.PublicId);

        await _cloudinary.DestroyAsync(deleteParams);

        await _unitOfWork.Media.DeleteAsync(id);

        return NoContent();
    }
}