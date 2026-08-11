using iERP.Modules.HR.Infrastructure;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Migrations.DesignTime;

public sealed class HrDbContextFactory : DesignTimeDbContextFactoryBase<HrDbContext>
{
    protected override HrDbContext Create(DbContextOptions<HrDbContext> options, ITenantContext tenantContext)
        => new(options, tenantContext);
}
