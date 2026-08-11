using iERP.Infrastructure.Persistence;
using iERP.Modules.Sales.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Sales.Infrastructure.Configurations;

public sealed class SalesQuotationLineConfiguration : AuditableEntityConfiguration<SalesQuotationLine>
{
    public override void Configure(EntityTypeBuilder<SalesQuotationLine> builder)
    {
        base.Configure(builder);
        builder.ToTable("sales_quotation_lines", "sales");
        builder.HasIndex(x => new { x.TenantId, x.Id });
    }
}
