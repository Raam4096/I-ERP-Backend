using iERP.SharedKernel.Primitives;

namespace iERP.Modules.AI.Domain;

public sealed class AIToolDefinition : AuditableEntity
{

    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string PermissionCode { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

}
