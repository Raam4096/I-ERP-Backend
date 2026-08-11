using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Identity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.Identity.Infrastructure.Configurations;

public sealed class FieldPermissionGrantConfiguration : AuditableEntityConfiguration<FieldPermissionGrant>
{
    public override void Configure(EntityTypeBuilder<FieldPermissionGrant> builder)
    {
        base.Configure(builder);
        builder.ToTable("field_permission_grants", "identity");
        builder.HasIndex(x => new { x.TenantId, x.RoleId, x.EntityName, x.FieldKey }).IsUnique();
    }
}
