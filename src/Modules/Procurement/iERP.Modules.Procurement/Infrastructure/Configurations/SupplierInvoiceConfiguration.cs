using iERP.Infrastructure.Persistence;
using iERP.Modules.Procurement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Procurement.Infrastructure.Configurations;

public sealed class SupplierInvoiceConfiguration : AuditableEntityConfiguration<SupplierInvoice>
{
    public override void Configure(EntityTypeBuilder<SupplierInvoice> builder)
    {
        base.Configure(builder);
        builder.ToTable("supplier_invoices", "procurement");
        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.DocumentNo }).IsUnique();
    }
}
