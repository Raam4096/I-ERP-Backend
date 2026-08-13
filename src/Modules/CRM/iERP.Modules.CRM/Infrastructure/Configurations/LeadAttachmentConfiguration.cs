using iERP.Modules.CRM.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.CRM.Infrastructure.Configurations;

public sealed class LeadAttachmentConfiguration : IEntityTypeConfiguration<LeadAttachment>
{
    public void Configure(EntityTypeBuilder<LeadAttachment> builder)
    {
        builder.ToTable("lead_attachments", "crm");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.FileName).HasMaxLength(512).IsRequired();
        builder.Property(x => x.FilePath).HasMaxLength(1024).IsRequired();
        builder.Property(x => x.ContentType).HasMaxLength(256).IsRequired();
        builder.Property(x => x.FileSize).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.HasIndex(x => x.FollowUpId);
    }
}
