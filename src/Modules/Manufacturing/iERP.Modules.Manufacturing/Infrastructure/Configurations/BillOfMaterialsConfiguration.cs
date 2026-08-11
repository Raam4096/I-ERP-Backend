using iERP.Infrastructure.Persistence;
using iERP.Modules.Manufacturing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Manufacturing.Infrastructure.Configurations;

public sealed class BillOfMaterialsConfiguration : AuditableEntityConfiguration<BillOfMaterials>
{
    public override void Configure(EntityTypeBuilder<BillOfMaterials> builder)
    {
        base.Configure(builder);
        builder.ToTable("bills_of_materials", "manufacturing");
        builder.HasIndex(x => new { x.TenantId, x.ItemId, x.BomVersion }).IsUnique();
    }
}
