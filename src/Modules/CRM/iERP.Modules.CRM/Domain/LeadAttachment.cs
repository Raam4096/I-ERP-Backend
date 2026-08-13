using iERP.SharedKernel.Primitives;

namespace iERP.Modules.CRM.Domain;

/// <summary>
/// Attachment metadata only. Binary content lives in file storage (future).
/// </summary>
public sealed class LeadAttachment : Entity
{
    private LeadAttachment()
    {
    }

    public Guid FollowUpId { get; private set; }
    public LeadFollowUp? FollowUp { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string FilePath { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public static LeadAttachment Create(
        Guid followUpId,
        string fileName,
        string filePath,
        string contentType,
        long fileSize,
        DateTimeOffset createdAt)
    {
        return new LeadAttachment
        {
            FollowUpId = followUpId,
            FileName = fileName.Trim(),
            FilePath = filePath.Trim(),
            ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType.Trim(),
            FileSize = fileSize < 0 ? 0 : fileSize,
            CreatedAt = createdAt
        };
    }
}
