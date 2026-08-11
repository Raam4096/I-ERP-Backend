using iERP.Infrastructure.Persistence;
using iERP.Modules.Assets.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Assets.Infrastructure.Configurations;

public sealed class AssetTypeConfiguration : AuditableEntityConfiguration<AssetType>
{
    public override void Configure(EntityTypeBuilder<AssetType> builder)
    {
        base.Configure(builder);
        builder.ToTable("asset_types", "assets");
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
    }
}
