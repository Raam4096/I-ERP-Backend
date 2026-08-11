using iERP.Infrastructure.Persistence;
using iERP.Modules.Procurement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Procurement.Infrastructure.Configurations;

public sealed class PurchaseOrderLineConfiguration : AuditableEntityConfiguration<PurchaseOrderLine>
{
    public override void Configure(EntityTypeBuilder<PurchaseOrderLine> builder)
    {
        base.Configure(builder);
        builder.ToTable("purchase_order_lines", "procurement");

    }
}
