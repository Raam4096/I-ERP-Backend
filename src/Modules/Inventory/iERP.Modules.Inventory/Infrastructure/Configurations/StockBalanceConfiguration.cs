using iERP.Infrastructure.Persistence;
using iERP.Modules.Inventory.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Inventory.Infrastructure.Configurations;

public sealed class StockBalanceConfiguration : AuditableEntityConfiguration<StockBalance>
{
    public override void Configure(EntityTypeBuilder<StockBalance> builder)
    {
        base.Configure(builder);
        builder.ToTable("stock_balances", "inventory");
        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.WarehouseId, x.BinLocationId, x.ItemId }).IsUnique();
    }
}
