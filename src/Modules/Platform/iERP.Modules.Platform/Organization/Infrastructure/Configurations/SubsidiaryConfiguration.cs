using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Organization.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.Organization.Infrastructure.Configurations;

public sealed class SubsidiaryConfiguration : AuditableEntityConfiguration<Subsidiary>
{
    public override void Configure(EntityTypeBuilder<Subsidiary> builder)
    {
        base.Configure(builder);
        builder.ToTable("subsidiaries", "organization");
        builder.Property(x => x.Code).HasMaxLength(64);
        builder.Property(x => x.Name).HasMaxLength(256);
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
    }
}
