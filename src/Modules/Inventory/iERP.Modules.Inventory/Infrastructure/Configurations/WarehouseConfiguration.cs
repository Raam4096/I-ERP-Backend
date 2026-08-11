using iERP.Infrastructure.Persistence;
using iERP.Modules.Inventory.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Inventory.Infrastructure.Configurations;

public sealed class WarehouseConfiguration : AuditableEntityConfiguration<Warehouse>
{
    public override void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        base.Configure(builder);
        builder.ToTable("warehouses", "inventory");
        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.Code }).IsUnique();
    }
}
