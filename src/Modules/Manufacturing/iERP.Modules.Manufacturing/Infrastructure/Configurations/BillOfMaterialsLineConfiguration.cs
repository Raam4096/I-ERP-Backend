using iERP.Infrastructure.Persistence;
using iERP.Modules.Manufacturing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Manufacturing.Infrastructure.Configurations;

public sealed class BillOfMaterialsLineConfiguration : AuditableEntityConfiguration<BillOfMaterialsLine>
{
    public override void Configure(EntityTypeBuilder<BillOfMaterialsLine> builder)
    {
        base.Configure(builder);
        builder.ToTable("bill_of_materials_lines", "manufacturing");

    }
}
