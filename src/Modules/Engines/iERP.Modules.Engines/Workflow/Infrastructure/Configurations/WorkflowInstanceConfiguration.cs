using iERP.Infrastructure.Persistence;
using iERP.Modules.Engines.Workflow.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Engines.Workflow.Infrastructure.Configurations;

public sealed class WorkflowInstanceConfiguration : AuditableEntityConfiguration<WorkflowInstance>
{
    public override void Configure(EntityTypeBuilder<WorkflowInstance> builder)
    {
        base.Configure(builder);
        builder.ToTable("workflow_instances", "workflow");
        builder.HasIndex(x => new { x.TenantId, x.EntityName, x.RecordId });
    }
}
