using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.DynamicModules.Domain;

public sealed class DynamicRecord : AuditableEntity
{

    public Guid DynamicEntityDefinitionId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = "{}";

}
