using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.Identity.Domain;

public sealed class RolePermission : AuditableEntity
{

    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }

}
