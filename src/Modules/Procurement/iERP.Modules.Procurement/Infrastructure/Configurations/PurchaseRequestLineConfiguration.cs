using iERP.Infrastructure.Persistence;
using iERP.Modules.Procurement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Procurement.Infrastructure.Configurations;

public sealed class PurchaseRequestLineConfiguration : AuditableEntityConfiguration<PurchaseRequestLine>
{
    public override void Configure(EntityTypeBuilder<PurchaseRequestLine> builder)
    {
        base.Configure(builder);
        builder.ToTable("purchase_request_lines", "procurement");

    }
}
