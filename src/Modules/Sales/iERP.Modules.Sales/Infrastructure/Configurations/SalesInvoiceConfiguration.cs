using iERP.Infrastructure.Persistence;
using iERP.Modules.Sales.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Sales.Infrastructure.Configurations;

public sealed class SalesInvoiceConfiguration : AuditableEntityConfiguration<SalesInvoice>
{
    public override void Configure(EntityTypeBuilder<SalesInvoice> builder)
    {
        base.Configure(builder);
        builder.ToTable("sales_invoices", "sales");
        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.DocumentNo }).IsUnique();
    }
}
