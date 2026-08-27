using iERP.Modules.Platform.Identity.Domain;
using iERP.Modules.Platform.Identity.Infrastructure;
using iERP.Application.Abstractions.Seeding;
using iERP.Modules.Platform.Tenancy.Infrastructure;
using iERP.SharedKernel.Security;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace iERP.Modules.Platform.Identity.Application.Seeding;

/// <summary>
/// Ensures ProcessFlow v4 default system roles exist for every tenant (startup, local + Railway).
/// </summary>
public sealed class SystemRoleSeeder : IDataSeeder
{
    private readonly IdentityDbContext _db;
    private readonly PlatformDbContext _platformDb;
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<SystemRoleSeeder> _logger;

    public SystemRoleSeeder(
        IdentityDbContext db,
        PlatformDbContext platformDb,
        ITenantContext tenantContext,
        ILogger<SystemRoleSeeder> logger)
    {
        _db = db;
        _platformDb = platformDb;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var tenantIds = await _platformDb.Tenants
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        if (tenantIds.Count == 0 && _tenantContext.HasTenant && _tenantContext.TenantId is Guid single)
        {
            tenantIds.Add(single);
        }

        if (tenantIds.Count == 0)
        {
            return;
        }

        foreach (var tenantId in tenantIds)
        {
            _tenantContext.SetTenant(tenantId);
            await SeedTenantRolesAsync(tenantId, cancellationToken);
        }
    }

    private async Task SeedTenantRolesAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        // Include soft-deleted rows: unique index (tenant_id, name) still applies.
        var existingRoles = await _db.Roles
            .IgnoreQueryFilters()
            .Where(x => x.TenantId == tenantId)
            .ToListAsync(cancellationToken);
        var byName = existingRoles
            .GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.OrderBy(x => x.IsDeleted).First(), StringComparer.OrdinalIgnoreCase);
        var added = 0;

        foreach (var roleName in SystemRoles.All)
        {
            if (byName.TryGetValue(roleName, out var existing))
            {
                if (existing.IsDeleted)
                {
                    existing.IsDeleted = false;
                    existing.DeletedAt = null;
                    existing.DeletedBy = null;
                }

                if (!existing.IsSystemRole)
                {
                    existing.IsSystemRole = true;
                }

                continue;
            }

            var role = new AppRole
            {
                Name = roleName,
                Description = $"{roleName} (system)",
                IsSystemRole = true
            };
            role.SetTenantId(tenantId);
            _db.Roles.Add(role);
            added++;
        }

        if (added > 0 || _db.ChangeTracker.HasChanges())
        {
            await _db.SaveChangesAsync(cancellationToken);
            if (added > 0)
            {
                _logger.LogInformation("Seeded {Count} system roles for tenant {TenantId}", added, tenantId);
            }
        }
    }
}
