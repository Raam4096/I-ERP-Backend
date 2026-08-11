using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.DynamicModules.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.DynamicModules.Infrastructure.Configurations;

public sealed class DynamicFieldDefinitionConfiguration : AuditableEntityConfiguration<DynamicFieldDefinition>
{
    public override void Configure(EntityTypeBuilder<DynamicFieldDefinition> builder)
    {
        base.Configure(builder);
        builder.ToTable("dynamic_field_definitions", "dynamic");
        builder.HasIndex(x => new { x.TenantId, x.DynamicEntityDefinitionId, x.FieldKey }).IsUnique();
    }
}
