using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.Attachments.Domain;

public sealed class Attachment : AuditableEntity
{

    public string EntityName { get; set; } = string.Empty;
    public Guid RecordId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string BlobPath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public Guid UploadedBy { get; set; }

}
