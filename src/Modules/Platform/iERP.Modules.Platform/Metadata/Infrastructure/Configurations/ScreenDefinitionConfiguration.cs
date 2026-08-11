using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Metadata.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.Metadata.Infrastructure.Configurations;

public sealed class ScreenDefinitionConfiguration : AuditableEntityConfiguration<ScreenDefinition>
{
    public override void Configure(EntityTypeBuilder<ScreenDefinition> builder)
    {
        base.Configure(builder);
        builder.ToTable("screen_definitions", "metadata");
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
        builder.HasOne(x => x.ModuleDefinition).WithMany(x => x.Screens).HasForeignKey(x => x.ModuleDefinitionId);
    }
}
