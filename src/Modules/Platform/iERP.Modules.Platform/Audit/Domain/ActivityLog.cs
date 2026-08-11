using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.Audit.Domain;

public sealed class ActivityLog : AuditableEntity
{

    public Guid? UserId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public Guid RecordId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? IpAddress { get; set; }

}
