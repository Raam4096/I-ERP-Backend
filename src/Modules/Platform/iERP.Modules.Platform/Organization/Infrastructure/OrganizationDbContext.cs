using iERP.Infrastructure.Persistence;
using iERP.Modules.Platform.Organization.Domain;
using iERP.Modules.Platform.Settings.Domain;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Platform.Organization.Infrastructure;

public sealed class OrganizationDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public OrganizationDbContext(DbContextOptions<OrganizationDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Subsidiary> Subsidiaries => Set<Subsidiary>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<CostCenter> CostCenters => Set<CostCenter>();
    public DbSet<ReportingDimension> ReportingDimensions => Set<ReportingDimension>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<DocumentSequence> DocumentSequences => Set<DocumentSequence>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var assembly = typeof(OrganizationDbContext).Assembly;
        modelBuilder.HasDefaultSchema("organization");
        modelBuilder.ApplyConfigurationsFromNamespace(assembly, "iERP.Modules.Platform.Organization.Infrastructure.Configurations");
        modelBuilder.ApplyConfigurationsFromNamespace(assembly, "iERP.Modules.Platform.Settings.Infrastructure.Configurations");
        modelBuilder.ApplySnakeCaseNamingConvention();
        modelBuilder.ConfigureMoneyPrecision();
        modelBuilder.ApplyTenantAndSoftDeleteFilters(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }
}
