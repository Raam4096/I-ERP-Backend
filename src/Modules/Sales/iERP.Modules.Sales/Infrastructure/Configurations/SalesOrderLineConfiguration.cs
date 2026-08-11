using iERP.Infrastructure.Persistence;
using iERP.Modules.Sales.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Sales.Infrastructure.Configurations;

public sealed class SalesOrderLineConfiguration : AuditableEntityConfiguration<SalesOrderLine>
{
    public override void Configure(EntityTypeBuilder<SalesOrderLine> builder)
    {
        base.Configure(builder);
        builder.ToTable("sales_order_lines", "sales");
        builder.HasIndex(x => new { x.TenantId, x.Id });
    }
}
