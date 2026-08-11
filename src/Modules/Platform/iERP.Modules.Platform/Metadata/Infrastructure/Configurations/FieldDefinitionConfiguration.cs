using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Metadata.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.Metadata.Infrastructure.Configurations;

public sealed class FieldDefinitionConfiguration : AuditableEntityConfiguration<FieldDefinition>
{
    public override void Configure(EntityTypeBuilder<FieldDefinition> builder)
    {
        base.Configure(builder);
        builder.ToTable("field_definitions", "metadata");
        builder.HasOne(x => x.SectionDefinition).WithMany(x => x.Fields).HasForeignKey(x => x.SectionDefinitionId);
    }
}
