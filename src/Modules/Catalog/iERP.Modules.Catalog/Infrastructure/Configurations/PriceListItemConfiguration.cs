using iERP.Infrastructure.Persistence;
using iERP.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Catalog.Infrastructure.Configurations;

public sealed class PriceListItemConfiguration : AuditableEntityConfiguration<PriceListItem>
{
    public override void Configure(EntityTypeBuilder<PriceListItem> builder)
    {
        base.Configure(builder);
        builder.ToTable("price_list_items", "catalog");
        builder.HasIndex(x => new { x.TenantId, x.PriceListId, x.ItemId }).IsUnique();
    }
}
