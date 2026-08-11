using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Metadata.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.Metadata.Infrastructure.Configurations;

public sealed class CustomFieldValueConfiguration : AuditableEntityConfiguration<CustomFieldValue>
{
    public override void Configure(EntityTypeBuilder<CustomFieldValue> builder)
    {
        base.Configure(builder);
        builder.ToTable("custom_field_values", "metadata");
        builder.Property(x => x.ValueJson).HasColumnType("jsonb");
        builder.HasIndex(x => new { x.TenantId, x.EntityName, x.RecordId, x.FieldKey }).IsUnique();
    }
}
