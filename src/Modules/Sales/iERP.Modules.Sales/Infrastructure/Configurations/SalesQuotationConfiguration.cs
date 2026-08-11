using iERP.Infrastructure.Persistence;
using iERP.Modules.Sales.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Sales.Infrastructure.Configurations;

public sealed class SalesQuotationConfiguration : AuditableEntityConfiguration<SalesQuotation>
{
    public override void Configure(EntityTypeBuilder<SalesQuotation> builder)
    {
        base.Configure(builder);
        builder.ToTable("sales_quotations", "sales");
        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.DocumentNo }).IsUnique();
    }
}
