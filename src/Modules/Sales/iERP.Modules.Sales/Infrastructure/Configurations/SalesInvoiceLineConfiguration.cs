using iERP.Infrastructure.Persistence;
using iERP.Modules.Sales.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Sales.Infrastructure.Configurations;

public sealed class SalesInvoiceLineConfiguration : AuditableEntityConfiguration<SalesInvoiceLine>
{
    public override void Configure(EntityTypeBuilder<SalesInvoiceLine> builder)
    {
        base.Configure(builder);
        builder.ToTable("sales_invoice_lines", "sales");
        builder.HasIndex(x => new { x.TenantId, x.Id });
    }
}
