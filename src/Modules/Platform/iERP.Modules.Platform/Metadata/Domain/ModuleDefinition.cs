using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.Metadata.Domain;

public sealed class ModuleDefinition : AuditableEntity
{

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<ScreenDefinition> Screens { get; set; } = new List<ScreenDefinition>();

}
