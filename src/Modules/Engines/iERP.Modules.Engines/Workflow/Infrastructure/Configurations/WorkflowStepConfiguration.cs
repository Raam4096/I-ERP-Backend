using iERP.Infrastructure.Persistence;
using iERP.Modules.Engines.Workflow.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Engines.Workflow.Infrastructure.Configurations;

public sealed class WorkflowStepConfiguration : AuditableEntityConfiguration<WorkflowStep>
{
    public override void Configure(EntityTypeBuilder<WorkflowStep> builder)
    {
        base.Configure(builder);
        builder.ToTable("workflow_steps", "workflow");
        builder.HasIndex(x => new { x.TenantId, x.WorkflowDefinitionId, x.Code }).IsUnique();
    }
}
