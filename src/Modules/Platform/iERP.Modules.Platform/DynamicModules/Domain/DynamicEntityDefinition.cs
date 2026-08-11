using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.DynamicModules.Domain;

public sealed class DynamicEntityDefinition : AuditableEntity
{

    public Guid DynamicModuleDefinitionId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

}
