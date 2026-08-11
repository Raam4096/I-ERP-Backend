using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Metadata.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.Metadata.Infrastructure.Configurations;

public sealed class ModuleDefinitionConfiguration : AuditableEntityConfiguration<ModuleDefinition>
{
    public override void Configure(EntityTypeBuilder<ModuleDefinition> builder)
    {
        base.Configure(builder);
        builder.ToTable("module_definitions", "metadata");
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
    }
}
