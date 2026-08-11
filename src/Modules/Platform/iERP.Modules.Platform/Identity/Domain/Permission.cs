using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.Identity.Domain;

public sealed class Permission : AuditableEntity
{

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Module { get; set; }
    public string? Description { get; set; }

}
