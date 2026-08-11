using iERP.Modules.Manufacturing.Infrastructure;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Migrations.DesignTime;

public sealed class ManufacturingDbContextFactory : DesignTimeDbContextFactoryBase<ManufacturingDbContext>
{
    protected override ManufacturingDbContext Create(DbContextOptions<ManufacturingDbContext> options, ITenantContext tenantContext)
        => new(options, tenantContext);
}
