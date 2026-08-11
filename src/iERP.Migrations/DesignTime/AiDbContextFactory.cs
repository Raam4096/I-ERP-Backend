using iERP.Modules.AI.Infrastructure;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Migrations.DesignTime;

public sealed class AiDbContextFactory : DesignTimeDbContextFactoryBase<AiDbContext>
{
    protected override AiDbContext Create(DbContextOptions<AiDbContext> options, ITenantContext tenantContext)
        => new(options, tenantContext);
}
