using iERP.Infrastructure.Persistence;
using iERP.Modules.Inventory.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Inventory.Infrastructure.Configurations;

public sealed class StockTransferConfiguration : AuditableEntityConfiguration<StockTransfer>
{
    public override void Configure(EntityTypeBuilder<StockTransfer> builder)
    {
        base.Configure(builder);
        builder.ToTable("stock_transfers", "inventory");
        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.DocumentNo }).IsUnique();
    }
}
