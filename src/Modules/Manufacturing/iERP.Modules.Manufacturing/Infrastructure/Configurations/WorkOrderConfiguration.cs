using iERP.Infrastructure.Persistence;
using iERP.Modules.Manufacturing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Manufacturing.Infrastructure.Configurations;

public sealed class WorkOrderConfiguration : AuditableEntityConfiguration<WorkOrder>
{
    public override void Configure(EntityTypeBuilder<WorkOrder> builder)
    {
        base.Configure(builder);
        builder.ToTable("work_orders", "manufacturing");
        builder.HasIndex(x => new { x.TenantId, x.SubsidiaryId, x.DocumentNo }).IsUnique();
    }
}
