using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.Tenancy.Domain;

/// <summary>
/// SaaS customer root. Not tenant-scoped (no tenant_id on itself).
/// </summary>
public sealed class Tenant : Entity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = "active";
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
