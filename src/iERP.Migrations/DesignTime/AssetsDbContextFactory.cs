using iERP.Modules.Assets.Infrastructure;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Migrations.DesignTime;

public sealed class AssetsDbContextFactory : DesignTimeDbContextFactoryBase<AssetsDbContext>
{
    protected override AssetsDbContext Create(DbContextOptions<AssetsDbContext> options, ITenantContext tenantContext)
        => new(options, tenantContext);
}
