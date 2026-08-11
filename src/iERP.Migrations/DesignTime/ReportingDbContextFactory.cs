using iERP.Modules.Reporting.Infrastructure;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Migrations.DesignTime;

public sealed class ReportingDbContextFactory : DesignTimeDbContextFactoryBase<ReportingDbContext>
{
    protected override ReportingDbContext Create(DbContextOptions<ReportingDbContext> options, ITenantContext tenantContext)
        => new(options, tenantContext);
}
