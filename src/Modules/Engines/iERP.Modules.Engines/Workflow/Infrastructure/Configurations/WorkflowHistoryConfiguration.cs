using iERP.Infrastructure.Persistence;
using iERP.Modules.Engines.Workflow.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Engines.Workflow.Infrastructure.Configurations;

public sealed class WorkflowHistoryConfiguration : AuditableEntityConfiguration<WorkflowHistory>
{
    public override void Configure(EntityTypeBuilder<WorkflowHistory> builder)
    {
        base.Configure(builder);
        builder.ToTable("workflow_histories", "workflow");
        builder.HasIndex(x => new { x.TenantId, x.WorkflowInstanceId });
    }
}
