using iERP.Infrastructure.Persistence;
using iERP.Modules.Inventory.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Inventory.Infrastructure.Configurations;

public sealed class StockReservationConfiguration : AuditableEntityConfiguration<StockReservation>
{
    public override void Configure(EntityTypeBuilder<StockReservation> builder)
    {
        base.Configure(builder);
        builder.ToTable("stock_reservations", "inventory");
        builder.HasIndex(x => new { x.TenantId, x.SourceEntityName, x.SourceRecordId });
    }
}
