using iERP.Infrastructure.Persistence;
using iERP.Modules.Engines.Rules.Domain;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Engines.Rules.Infrastructure;

public sealed class RulesDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public RulesDbContext(DbContextOptions<RulesDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<RuleDefinition> RuleDefinitions => Set<RuleDefinition>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("rules");
        modelBuilder.ApplyConfigurationsFromNamespace(
            typeof(RulesDbContext).Assembly,
            "iERP.Modules.Engines.Rules.Infrastructure.Configurations");
        modelBuilder.ApplySnakeCaseNamingConvention();
        modelBuilder.ConfigureMoneyPrecision();
        modelBuilder.ApplyTenantAndSoftDeleteFilters(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }
}
