using iERP.Infrastructure.Persistence;
using iERP.Modules.Inventory.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Inventory.Infrastructure.Configurations;

public sealed class InventoryTransactionLineConfiguration : AuditableEntityConfiguration<InventoryTransactionLine>
{
    public override void Configure(EntityTypeBuilder<InventoryTransactionLine> builder)
    {
        base.Configure(builder);
        builder.ToTable("inventory_transaction_lines", "inventory");

    }
}
