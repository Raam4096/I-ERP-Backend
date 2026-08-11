using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.DynamicModules.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.DynamicModules.Infrastructure.Configurations;

public sealed class DynamicRecordConfiguration : AuditableEntityConfiguration<DynamicRecord>
{
    public override void Configure(EntityTypeBuilder<DynamicRecord> builder)
    {
        base.Configure(builder);
        builder.ToTable("dynamic_records", "dynamic");
        builder.Property(x => x.PayloadJson).HasColumnType("jsonb");
        builder.HasIndex(x => new { x.TenantId, x.EntityName });
    }
}
