using iERP.Infrastructure.Persistence;
using iERP.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Catalog.Infrastructure.Configurations;

public sealed class ItemCategoryConfiguration : AuditableEntityConfiguration<ItemCategory>
{
    public override void Configure(EntityTypeBuilder<ItemCategory> builder)
    {
        base.Configure(builder);
        builder.ToTable("item_categories", "catalog");
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
    }
}
