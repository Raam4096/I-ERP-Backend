using iERP.Infrastructure.Persistence;
using iERP.Modules.Manufacturing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Manufacturing.Infrastructure.Configurations;

public sealed class WorkOrderLineConfiguration : AuditableEntityConfiguration<WorkOrderLine>
{
    public override void Configure(EntityTypeBuilder<WorkOrderLine> builder)
    {
        base.Configure(builder);
        builder.ToTable("work_order_lines", "manufacturing");

    }
}
