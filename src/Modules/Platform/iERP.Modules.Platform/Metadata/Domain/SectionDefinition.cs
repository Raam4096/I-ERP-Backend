using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.Metadata.Domain;

public sealed class SectionDefinition : AuditableEntity
{

    public Guid ScreenDefinitionId { get; set; }
    public ScreenDefinition? ScreenDefinition { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public ICollection<FieldDefinition> Fields { get; set; } = new List<FieldDefinition>();

}
