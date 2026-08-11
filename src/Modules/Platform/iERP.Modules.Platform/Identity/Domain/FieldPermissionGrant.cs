using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.Identity.Domain;

public sealed class FieldPermissionGrant : AuditableEntity
{

    public Guid RoleId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string FieldKey { get; set; } = string.Empty;
    public bool CanView { get; set; } = true;
    public bool CanEdit { get; set; }

}
