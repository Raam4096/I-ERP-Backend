using iERP.Infrastructure.Persistence;
using iERP.Modules.Engines.Bridge.Domain;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Engines.Bridge.Infrastructure;

public sealed class BridgeDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public BridgeDbContext(DbContextOptions<BridgeDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<BridgeDefinition> BridgeDefinitions => Set<BridgeDefinition>();
    public DbSet<BridgeMapping> BridgeMappings => Set<BridgeMapping>();
    public DbSet<BridgeLog> BridgeLogs => Set<BridgeLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("bridge");
        modelBuilder.ApplyConfigurationsFromNamespace(
            typeof(BridgeDbContext).Assembly,
            "iERP.Modules.Engines.Bridge.Infrastructure.Configurations");
        modelBuilder.ApplySnakeCaseNamingConvention();
        modelBuilder.ConfigureMoneyPrecision();
        modelBuilder.ApplyTenantAndSoftDeleteFilters(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }
}
