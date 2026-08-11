using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Identity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.Identity.Infrastructure.Configurations;

public sealed class AppUserConfiguration : AuditableEntityConfiguration<AppUser>
{
    public override void Configure(EntityTypeBuilder<AppUser> builder)
    {
        base.Configure(builder);
        builder.ToTable("users", "identity");
        builder.Property(x => x.Email).HasMaxLength(256);
        builder.HasIndex(x => new { x.TenantId, x.Email }).IsUnique();
    }
}
