using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.Identity.Domain;

public sealed class AppUser : AuditableEntity
{

    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset? LastLoginAt { get; set; }

}
