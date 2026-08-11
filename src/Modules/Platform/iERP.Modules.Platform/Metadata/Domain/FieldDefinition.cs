using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.Metadata.Domain;

public sealed class FieldDefinition : AuditableEntity
{

    public Guid SectionDefinitionId { get; set; }
    public SectionDefinition? SectionDefinition { get; set; }
    public string FieldKey { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string DataType { get; set; } = "string";
    public string ControlType { get; set; } = "text";
    public int DisplayOrder { get; set; }
    public bool IsRequired { get; set; }
    public bool IsReadOnly { get; set; }
    public bool IsVisible { get; set; } = true;
    public int? Width { get; set; }

}
