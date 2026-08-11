using iERP.Infrastructure.Persistence;
using iERP.Modules.Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Catalog.Infrastructure.Configurations;

public sealed class UnitOfMeasureConfiguration : AuditableEntityConfiguration<UnitOfMeasure>
{
    public override void Configure(EntityTypeBuilder<UnitOfMeasure> builder)
    {
        base.Configure(builder);
        builder.ToTable("units_of_measure", "catalog");
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
    }
}
