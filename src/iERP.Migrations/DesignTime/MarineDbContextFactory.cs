using iERP.Modules.Marine.Infrastructure;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Migrations.DesignTime;

public sealed class MarineDbContextFactory : DesignTimeDbContextFactoryBase<MarineDbContext>
{
    protected override MarineDbContext Create(DbContextOptions<MarineDbContext> options, ITenantContext tenantContext)
        => new(options, tenantContext);
}
