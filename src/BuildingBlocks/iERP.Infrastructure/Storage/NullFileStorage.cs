using iERP.Application.Abstractions.Storage;
using Microsoft.Extensions.Logging;

namespace iERP.Infrastructure.Storage;

public sealed class NullFileStorage : IFileStorage
{
    private readonly ILogger<NullFileStorage> _logger;

    public NullFileStorage(ILogger<NullFileStorage> logger)
    {
        _logger = logger;
    }

    public Task<string> UploadAsync(Stream content, string blobPath, string contentType, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("NullFileStorage upload stub for {BlobPath}", blobPath);
        return Task.FromResult(blobPath);
    }

    public Task<Stream> DownloadAsync(string blobPath, CancellationToken cancellationToken = default) =>
        Task.FromResult<Stream>(new MemoryStream());

    public Task DeleteAsync(string blobPath, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
