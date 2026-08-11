using iERP.Infrastructure.Persistence;
using iERP.Modules.Projects.Domain;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;

namespace iERP.Modules.Projects.Infrastructure;

public sealed class ProjectsDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public ProjectsDbContext(DbContextOptions<ProjectsDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<RetentionRule> RetentionRules => Set<RetentionRule>();
    public DbSet<Subcontractor> Subcontractors => Set<Subcontractor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("projects");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProjectsDbContext).Assembly);
        modelBuilder.ApplySnakeCaseNamingConvention();
        modelBuilder.ConfigureMoneyPrecision();
        modelBuilder.ApplyTenantAndSoftDeleteFilters(_tenantContext);
        base.OnModelCreating(modelBuilder);
    }
}
