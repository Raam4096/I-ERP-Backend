using iERP.Infrastructure.Persistence;
using iERP.Modules.Engines.Workflow.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace iERP.Modules.Engines.Workflow.Infrastructure.Configurations;

public sealed class WorkflowDefinitionConfiguration : AuditableEntityConfiguration<WorkflowDefinition>
{
    public override void Configure(EntityTypeBuilder<WorkflowDefinition> builder)
    {
        base.Configure(builder);
        builder.ToTable("workflow_definitions", "workflow");
        builder.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();
    }
}
