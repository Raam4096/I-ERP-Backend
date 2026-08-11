using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.Identity.Domain;

public sealed class UserRole : AuditableEntity
{

    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

}
