using iERP.Modules.Sales.Infrastructure;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Migrations.DesignTime;

public sealed class SalesDbContextFactory : DesignTimeDbContextFactoryBase<SalesDbContext>
{
    protected override SalesDbContext Create(DbContextOptions<SalesDbContext> options, ITenantContext tenantContext)
        => new(options, tenantContext);
}
