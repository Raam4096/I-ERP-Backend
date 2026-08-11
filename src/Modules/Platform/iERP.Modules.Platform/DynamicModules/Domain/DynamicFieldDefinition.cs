using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.DynamicModules.Domain;

public sealed class DynamicFieldDefinition : AuditableEntity
{

    public Guid DynamicEntityDefinitionId { get; set; }
    public string FieldKey { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string DataType { get; set; } = "string";
    public int DisplayOrder { get; set; }
    public bool IsRequired { get; set; }

}
