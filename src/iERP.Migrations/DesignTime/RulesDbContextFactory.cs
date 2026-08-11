using iERP.Modules.Engines.Rules.Infrastructure;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Migrations.DesignTime;

public sealed class RulesDbContextFactory : DesignTimeDbContextFactoryBase<RulesDbContext>
{
    protected override RulesDbContext Create(DbContextOptions<RulesDbContext> options, ITenantContext tenantContext)
        => new(options, tenantContext);
}
