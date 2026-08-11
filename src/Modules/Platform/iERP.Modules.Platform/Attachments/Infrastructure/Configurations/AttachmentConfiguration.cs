using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Attachments.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Platform.Attachments.Infrastructure.Configurations;

public sealed class AttachmentConfiguration : AuditableEntityConfiguration<Attachment>
{
    public override void Configure(EntityTypeBuilder<Attachment> builder)
    {
        base.Configure(builder);
        builder.ToTable("attachments", "attachments");
        builder.HasIndex(x => new { x.TenantId, x.EntityName, x.RecordId });
    }
}
