using iERP.Infrastructure.Persistence;
using iERP.Modules.Procurement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Procurement.Infrastructure.Configurations;

public sealed class SupplierInvoiceLineConfiguration : AuditableEntityConfiguration<SupplierInvoiceLine>
{
    public override void Configure(EntityTypeBuilder<SupplierInvoiceLine> builder)
    {
        base.Configure(builder);
        builder.ToTable("supplier_invoice_lines", "procurement");

    }
}
