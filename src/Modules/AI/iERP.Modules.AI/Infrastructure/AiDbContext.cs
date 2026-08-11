using iERP.Infrastructure.Persistence;
using iERP.Modules.AI.Domain;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.AI.Infrastructure;

public sealed class AiDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public AiDbContext(DbContextOptions<AiDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<AIToolDefinition> AIToolDefinitions => Set<AIToolDefinition>();
    public DbSet<AIToolPermission> AIToolPermissions => Set<AIToolPermission>();
    public DbSet<AILog> AILogs => Set<AILog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("ai");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AiDbContext).Assembly);
        modelBuilder.ApplySnakeCaseNamingConvention();
        modelBuilder.ConfigureMoneyPrecision();
        modelBuilder.ApplyTenantAndSoftDeleteFilters(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }
}
