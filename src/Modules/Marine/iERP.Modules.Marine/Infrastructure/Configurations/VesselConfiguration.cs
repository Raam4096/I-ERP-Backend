using iERP.Infrastructure.Persistence;
using iERP.Modules.Marine.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Marine.Infrastructure.Configurations;

public sealed class VesselConfiguration : AuditableEntityConfiguration<Vessel>
{
    public override void Configure(EntityTypeBuilder<Vessel> builder)
    {
        base.Configure(builder);
        builder.ToTable("vessels", "marine");
        builder.HasIndex(x => new { x.TenantId, x.VesselCode }).IsUnique();
    }
}
