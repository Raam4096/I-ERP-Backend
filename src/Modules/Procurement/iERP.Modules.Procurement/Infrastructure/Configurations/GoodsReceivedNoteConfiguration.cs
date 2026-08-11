using iERP.Infrastructure.Persistence;
using iERP.Modules.Procurement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Procurement.Infrastructure.Configurations;

public sealed class GoodsReceivedNoteConfiguration : AuditableEntityConfiguration<GoodsReceivedNote>
{
    public override void Configure(EntityTypeBuilder<GoodsReceivedNote> builder)
    {
        base.Configure(builder);
        builder.ToTable("goods_received_notes", "procurement");
        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.DocumentNo }).IsUnique();
    }
}
