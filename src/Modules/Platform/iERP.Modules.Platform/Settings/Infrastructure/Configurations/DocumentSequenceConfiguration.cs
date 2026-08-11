using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Settings.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.Settings.Infrastructure.Configurations;

public sealed class DocumentSequenceConfiguration : AuditableEntityConfiguration<DocumentSequence>
{
    public override void Configure(EntityTypeBuilder<DocumentSequence> builder)
    {
        base.Configure(builder);
        builder.ToTable("document_sequences", "organization");
        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.EntityName }).IsUnique();
        builder.Property(x => x.EntityName).HasMaxLength(128);
    }
}
