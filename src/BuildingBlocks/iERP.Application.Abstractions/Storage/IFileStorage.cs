namespace iERP.Application.Abstractions.Storage;

public interface IFileStorage
{
    Task<string> UploadAsync(
        Stream content,
        string blobPath,
        string contentType,
        CancellationToken cancellationToken = default);

    Task<Stream> DownloadAsync(string blobPath, CancellationToken cancellationToken = default);

    Task DeleteAsync(string blobPath, CancellationToken cancellationToken = default);
}
