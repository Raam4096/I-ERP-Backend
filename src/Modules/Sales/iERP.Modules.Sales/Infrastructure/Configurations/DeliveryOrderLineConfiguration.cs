using iERP.Infrastructure.Persistence;
using iERP.Modules.Sales.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Sales.Infrastructure.Configurations;

public sealed class DeliveryOrderLineConfiguration : AuditableEntityConfiguration<DeliveryOrderLine>
{
    public override void Configure(EntityTypeBuilder<DeliveryOrderLine> builder)
    {
        base.Configure(builder);
        builder.ToTable("delivery_order_lines", "sales");
        builder.HasIndex(x => new { x.TenantId, x.Id });
    }
}
