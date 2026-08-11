using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.DynamicModules.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.DynamicModules.Infrastructure.Configurations;

public sealed class DynamicModuleDefinitionConfiguration : AuditableEntityConfiguration<DynamicModuleDefinition>
{
    public override void Configure(EntityTypeBuilder<DynamicModuleDefinition> builder)
    {
        base.Configure(builder);
        builder.ToTable("dynamic_module_definitions", "dynamic");
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
    }
}
