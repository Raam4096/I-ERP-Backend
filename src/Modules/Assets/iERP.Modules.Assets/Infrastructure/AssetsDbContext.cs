using iERP.Infrastructure.Persistence;
using iERP.Modules.Assets.Domain;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Assets.Infrastructure;

public sealed class AssetsDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public AssetsDbContext(DbContextOptions<AssetsDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<AssetType> AssetTypes => Set<AssetType>();
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<AssetMaintenanceSchedule> AssetMaintenanceSchedules => Set<AssetMaintenanceSchedule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("assets");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssetsDbContext).Assembly);
        modelBuilder.ApplySnakeCaseNamingConvention();
        modelBuilder.ConfigureMoneyPrecision();
        modelBuilder.ApplyTenantAndSoftDeleteFilters(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }
}
