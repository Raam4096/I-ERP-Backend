using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.DynamicModules.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.DynamicModules.Infrastructure.Configurations;

public sealed class DynamicEntityDefinitionConfiguration : AuditableEntityConfiguration<DynamicEntityDefinition>
{
    public override void Configure(EntityTypeBuilder<DynamicEntityDefinition> builder)
    {
        base.Configure(builder);
        builder.ToTable("dynamic_entity_definitions", "dynamic");
        builder.HasIndex(x => new { x.TenantId, x.EntityName }).IsUnique();
    }
}
