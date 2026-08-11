using iERP.Modules.Engines.Bridge.Infrastructure;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Migrations.DesignTime;

public sealed class BridgeDbContextFactory : DesignTimeDbContextFactoryBase<BridgeDbContext>
{
    protected override BridgeDbContext Create(DbContextOptions<BridgeDbContext> options, ITenantContext tenantContext)
        => new(options, tenantContext);
}
