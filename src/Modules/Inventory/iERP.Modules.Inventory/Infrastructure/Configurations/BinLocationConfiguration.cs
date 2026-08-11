using iERP.Infrastructure.Persistence;
using iERP.Modules.Inventory.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Inventory.Infrastructure.Configurations;

public sealed class BinLocationConfiguration : AuditableEntityConfiguration<BinLocation>
{
    public override void Configure(EntityTypeBuilder<BinLocation> builder)
    {
        base.Configure(builder);
        builder.ToTable("bin_locations", "inventory");
        builder.HasIndex(x => new { x.TenantId, x.WarehouseId, x.Code }).IsUnique();
    }
}
