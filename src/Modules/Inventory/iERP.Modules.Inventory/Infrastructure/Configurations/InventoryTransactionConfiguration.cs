using iERP.Infrastructure.Persistence;
using iERP.Modules.Inventory.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Inventory.Infrastructure.Configurations;

public sealed class InventoryTransactionConfiguration : AuditableEntityConfiguration<InventoryTransaction>
{
    public override void Configure(EntityTypeBuilder<InventoryTransaction> builder)
    {
        base.Configure(builder);
        builder.ToTable("inventory_transactions", "inventory");
        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.DocumentNo }).IsUnique();
    }
}
