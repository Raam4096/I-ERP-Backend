using iERP.Infrastructure.Persistence;
using iERP.Modules.Inventory.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Inventory.Infrastructure.Configurations;

public sealed class StockTransferLineConfiguration : AuditableEntityConfiguration<StockTransferLine>
{
    public override void Configure(EntityTypeBuilder<StockTransferLine> builder)
    {
        base.Configure(builder);
        builder.ToTable("stock_transfer_lines", "inventory");

    }
}
