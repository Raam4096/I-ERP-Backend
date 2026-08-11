using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Identity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.Identity.Infrastructure.Configurations;

public sealed class RolePermissionConfiguration : AuditableEntityConfiguration<RolePermission>
{
    public override void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        base.Configure(builder);
        builder.ToTable("role_permissions", "identity");
        builder.HasIndex(x => new { x.TenantId, x.RoleId, x.PermissionId }).IsUnique();
    }
}
