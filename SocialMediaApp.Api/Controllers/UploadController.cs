using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Api.Services;

namespace SocialMediaApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UploadController : ControllerBase
{
    private readonly BlobService _blob;

    public UploadController(BlobService blob)
    {
        _blob = blob;
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file selected.");
        if (file.Length > 5 * 1024 * 1024)
            return BadRequest("Image must be under 5 MB.");

        var allowed = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        if (!allowed.Contains(file.ContentType))
            return BadRequest("Only JPG, PNG, GIF or WEBP images are allowed.");

        var url = await _blob.UploadAsync(file);
        return Ok(new { url });
    }
}