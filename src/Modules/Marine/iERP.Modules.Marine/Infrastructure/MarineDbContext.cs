using iERP.Infrastructure.Persistence;
using iERP.Modules.Marine.Domain;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Marine.Infrastructure;

public sealed class MarineDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public MarineDbContext(DbContextOptions<MarineDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<PortLocation> PortLocations => Set<PortLocation>();
    public DbSet<Vessel> Vessels => Set<Vessel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("marine");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MarineDbContext).Assembly);
        modelBuilder.ApplySnakeCaseNamingConvention();
        modelBuilder.ConfigureMoneyPrecision();
        modelBuilder.ApplyTenantAndSoftDeleteFilters(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }
}
