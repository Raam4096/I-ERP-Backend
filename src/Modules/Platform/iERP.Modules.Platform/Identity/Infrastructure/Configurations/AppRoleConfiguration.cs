using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Identity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.Identity.Infrastructure.Configurations;

public sealed class AppRoleConfiguration : AuditableEntityConfiguration<AppRole>
{
    public override void Configure(EntityTypeBuilder<AppRole> builder)
    {
        base.Configure(builder);
        builder.ToTable("roles", "identity");
        builder.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();
    }
}
