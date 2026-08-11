using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.Metadata.Domain;

public sealed class ScreenDefinition : AuditableEntity
{

    public Guid ModuleDefinitionId { get; set; }
    public ModuleDefinition? ModuleDefinition { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string RenderMode { get; set; } = "standard";
    public string EntityName { get; set; } = string.Empty;
    public string ApiBasePath { get; set; } = string.Empty;
    public bool WorkflowEnabled { get; set; }
    public bool PrintEnabled { get; set; }
    public bool AiEnabled { get; set; }
    public ICollection<SectionDefinition> Sections { get; set; } = new List<SectionDefinition>();

}
