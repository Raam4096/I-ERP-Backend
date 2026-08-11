using iERP.Infrastructure.Persistence;
using iERP.Modules.Sales.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Sales.Infrastructure.Configurations;

public sealed class DeliveryOrderConfiguration : AuditableEntityConfiguration<DeliveryOrder>
{
    public override void Configure(EntityTypeBuilder<DeliveryOrder> builder)
    {
        base.Configure(builder);
        builder.ToTable("delivery_orders", "sales");
        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.DocumentNo }).IsUnique();
    }
}
