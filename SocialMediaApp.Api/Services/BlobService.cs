using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace SocialMediaApp.Api.Services;

public class BlobService
{
    private readonly BlobContainerClient _container;

    public BlobService(IConfiguration config)
    {
        var client = new BlobServiceClient(config["BlobStorage:ConnectionString"]);
        _container = client.GetBlobContainerClient(config["BlobStorage:ContainerName"]);
        _container.CreateIfNotExists(PublicAccessType.Blob);
    }

    public async Task<string> UploadAsync(IFormFile file)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var blob = _container.GetBlobClient($"{Guid.NewGuid()}{ext}");

        using var stream = file.OpenReadStream();
        await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });

        return blob.Uri.ToString();
    }
}