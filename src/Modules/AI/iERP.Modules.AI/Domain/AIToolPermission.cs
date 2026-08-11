using iERP.SharedKernel.Primitives;

namespace iERP.Modules.AI.Domain;

public sealed class AIToolPermission : AuditableEntity
{

    public Guid AIToolDefinitionId { get; set; }
    public Guid RoleId { get; set; }
    public string AllowedExecutionMode { get; set; } = "advisory";

}
