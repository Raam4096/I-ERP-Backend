using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Identity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.Identity.Infrastructure.Configurations;

public sealed class UserRoleConfiguration : AuditableEntityConfiguration<UserRole>
{
    public override void Configure(EntityTypeBuilder<UserRole> builder)
    {
        base.Configure(builder);
        builder.ToTable("user_roles", "identity");
        builder.HasIndex(x => new { x.TenantId, x.UserId, x.RoleId }).IsUnique();
    }
}
