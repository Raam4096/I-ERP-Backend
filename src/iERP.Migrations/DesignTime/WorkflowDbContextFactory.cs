using iERP.Modules.Engines.Workflow.Infrastructure;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Migrations.DesignTime;

public sealed class WorkflowDbContextFactory : DesignTimeDbContextFactoryBase<WorkflowDbContext>
{
    protected override WorkflowDbContext Create(DbContextOptions<WorkflowDbContext> options, ITenantContext tenantContext)
        => new(options, tenantContext);
}
