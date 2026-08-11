using iERP.Infrastructure.Persistence;
using iERP.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Catalog.Infrastructure.Configurations;

public sealed class ItemConfiguration : AuditableEntityConfiguration<Item>
{
    public override void Configure(EntityTypeBuilder<Item> builder)
    {
        base.Configure(builder);
        builder.ToTable("items", "catalog");
        builder.HasIndex(x => new { x.TenantId, x.ItemCode }).IsUnique();
    }
}
