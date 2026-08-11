using Azure.Storage.Blobs;
using iERP.Application.Abstractions.Options;
using iERP.Application.Abstractions.Storage;
using Microsoft.Extensions.Options;

namespace iERP.Infrastructure.Storage;

public sealed class AzureBlobFileStorage : IFileStorage
{
    private readonly BlobContainerClient _container;

    public AzureBlobFileStorage(IOptions<AzureBlobStorageOptions> options)
    {
        var value = options.Value;
        var service = new BlobServiceClient(value.ConnectionString);
        _container = service.GetBlobContainerClient(value.ContainerName);
    }

    public async Task<string> UploadAsync(Stream content, string blobPath, string contentType, CancellationToken cancellationToken = default)
    {
        var client = _container.GetBlobClient(blobPath);
        await client.UploadAsync(content, overwrite: true, cancellationToken);
        return blobPath;
    }

    public async Task<Stream> DownloadAsync(string blobPath, CancellationToken cancellationToken = default)
    {
        var client = _container.GetBlobClient(blobPath);
        var response = await client.DownloadStreamingAsync(cancellationToken: cancellationToken);
        return response.Value.Content;
    }

    public async Task DeleteAsync(string blobPath, CancellationToken cancellationToken = default)
    {
        var client = _container.GetBlobClient(blobPath);
        await client.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }
}
