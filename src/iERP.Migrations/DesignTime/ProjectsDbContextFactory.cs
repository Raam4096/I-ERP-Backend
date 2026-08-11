using iERP.Modules.Projects.Infrastructure;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Migrations.DesignTime;

public sealed class ProjectsDbContextFactory : DesignTimeDbContextFactoryBase<ProjectsDbContext>
{
    protected override ProjectsDbContext Create(DbContextOptions<ProjectsDbContext> options, ITenantContext tenantContext)
        => new(options, tenantContext);
}
