using iERP.Infrastructure.Persistence;
using iERP.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Catalog.Infrastructure.Configurations;

public sealed class PriceListConfiguration : AuditableEntityConfiguration<PriceList>
{
    public override void Configure(EntityTypeBuilder<PriceList> builder)
    {
        base.Configure(builder);
        builder.ToTable("price_lists", "catalog");
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
    }
}
