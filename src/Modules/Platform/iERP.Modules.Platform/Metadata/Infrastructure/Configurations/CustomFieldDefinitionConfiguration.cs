using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Metadata.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.Metadata.Infrastructure.Configurations;

public sealed class CustomFieldDefinitionConfiguration : AuditableEntityConfiguration<CustomFieldDefinition>
{
    public override void Configure(EntityTypeBuilder<CustomFieldDefinition> builder)
    {
        base.Configure(builder);
        builder.ToTable("custom_field_definitions", "metadata");
        builder.HasIndex(x => new { x.TenantId, x.EntityName, x.FieldKey }).IsUnique();
    }
}
