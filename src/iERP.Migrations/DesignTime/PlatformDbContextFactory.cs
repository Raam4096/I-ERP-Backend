using iERP.Modules.Platform.Tenancy.Infrastructure;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Migrations.DesignTime;

public sealed class PlatformDbContextFactory : DesignTimeDbContextFactoryBase<PlatformDbContext>
{
    protected override PlatformDbContext Create(DbContextOptions<PlatformDbContext> options, ITenantContext tenantContext)
        => new(options, tenantContext);
}
