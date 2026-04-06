using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

public class BlobService
{
    private readonly BlobContainerClient _containerClient;

    public BlobService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureBlobStorage:ConnectionString"];
        var containerName = configuration["AzureBlobStorage:ContainerName"];

        var blobServiceClient = new BlobServiceClient(connectionString);
        _containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        _containerClient.CreateIfNotExists();
    }

    public async Task<string> UploadAsync(IFormFile file)
    {
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

        var blobClient = _containerClient.GetBlobClient(fileName);

        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, new BlobHttpHeaders
        {
            ContentType = file.ContentType
        });

        return blobClient.Uri.ToString();
    }
}