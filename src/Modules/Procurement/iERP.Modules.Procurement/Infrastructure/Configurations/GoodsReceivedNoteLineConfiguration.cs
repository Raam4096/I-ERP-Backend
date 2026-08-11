using iERP.Infrastructure.Persistence;
using iERP.Modules.Procurement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Procurement.Infrastructure.Configurations;

public sealed class GoodsReceivedNoteLineConfiguration : AuditableEntityConfiguration<GoodsReceivedNoteLine>
{
    public override void Configure(EntityTypeBuilder<GoodsReceivedNoteLine> builder)
    {
        base.Configure(builder);
        builder.ToTable("goods_received_note_lines", "procurement");

    }
}
