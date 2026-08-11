using iERP.Infrastructure.Persistence;
using iERP.Modules.Assets.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Assets.Infrastructure.Configurations;

public sealed class AssetConfiguration : AuditableEntityConfiguration<Asset>
{
    public override void Configure(EntityTypeBuilder<Asset> builder)
    {
        base.Configure(builder);
        builder.ToTable("assets", "assets");
        builder.HasIndex(x => new { x.TenantId, x.AssetCode }).IsUnique();
    }
}
