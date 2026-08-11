using iERP.Infrastructure.Persistence;
using iERP.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Catalog.Infrastructure.Configurations;

public sealed class UnitOfMeasureConversionConfiguration : AuditableEntityConfiguration<UnitOfMeasureConversion>
{
    public override void Configure(EntityTypeBuilder<UnitOfMeasureConversion> builder)
    {
        base.Configure(builder);
        builder.ToTable("unit_of_measure_conversions", "catalog");
        builder.HasIndex(x => new { x.TenantId, x.FromUomId, x.ToUomId }).IsUnique();
    }
}
