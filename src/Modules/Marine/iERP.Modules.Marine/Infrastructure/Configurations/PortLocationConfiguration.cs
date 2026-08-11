using iERP.Infrastructure.Persistence;
using iERP.Modules.Marine.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Marine.Infrastructure.Configurations;

public sealed class PortLocationConfiguration : AuditableEntityConfiguration<PortLocation>
{
    public override void Configure(EntityTypeBuilder<PortLocation> builder)
    {
        base.Configure(builder);
        builder.ToTable("port_locations", "marine");
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
    }
}
