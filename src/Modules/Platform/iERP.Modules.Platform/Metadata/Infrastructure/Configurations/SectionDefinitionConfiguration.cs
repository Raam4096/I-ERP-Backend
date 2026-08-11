using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Metadata.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.Metadata.Infrastructure.Configurations;

public sealed class SectionDefinitionConfiguration : AuditableEntityConfiguration<SectionDefinition>
{
    public override void Configure(EntityTypeBuilder<SectionDefinition> builder)
    {
        base.Configure(builder);
        builder.ToTable("section_definitions", "metadata");
        builder.HasOne(x => x.ScreenDefinition).WithMany(x => x.Sections).HasForeignKey(x => x.ScreenDefinitionId);
    }
}
