using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.Metadata.Domain;

public sealed class CustomFieldDefinition : AuditableEntity
{

    public string EntityName { get; set; } = string.Empty;
    public string FieldKey { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string DataType { get; set; } = "string";
    public int DisplayOrder { get; set; }
    public bool IsRequired { get; set; }
    public bool IsActive { get; set; } = true;

}
