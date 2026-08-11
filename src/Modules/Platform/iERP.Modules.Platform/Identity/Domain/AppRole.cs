using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.Identity.Domain;

public sealed class AppRole : AuditableEntity
{

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }

}
