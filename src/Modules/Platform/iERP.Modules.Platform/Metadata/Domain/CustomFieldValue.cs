using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.Metadata.Domain;

public sealed class CustomFieldValue : AuditableEntity
{

    public string EntityName { get; set; } = string.Empty;
    public Guid RecordId { get; set; }
    public string FieldKey { get; set; } = string.Empty;
    public string? ValueText { get; set; }
    public decimal? ValueNumber { get; set; }
    public DateTimeOffset? ValueDate { get; set; }
    public bool? ValueBoolean { get; set; }
    public string? ValueJson { get; set; }

}
