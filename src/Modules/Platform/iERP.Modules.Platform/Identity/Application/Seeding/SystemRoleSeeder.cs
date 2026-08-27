using iERP.Modules.Platform.Identity.Domain;
using iERP.Modules.Platform.Identity.Infrastructure;
using iERP.Application.Abstractions.Seeding;
using iERP.SharedKernel.Security;
using iERP.SharedKernel.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace iERP.Modules.Platform.Identity.Application.Seeding;

/// <summary>
/// Ensures ProcessFlow v4 default system roles exist for the current tenant.
/// </summary>
public sealed class SystemRoleSeeder : IDataSeeder
{
    private readonly IdentityDbContext _db;
    private readonly ITenantContext _tenantContext;
    private readonly ILogger<SystemRoleSeeder> _logger;

    public SystemRoleSeeder(
        IdentityDbContext db,
        ITenantContext tenantContext,
        ILogger<SystemRoleSeeder> logger)
    {
        _db = db;
        _tenantContext = tenantContext;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!_tenantContext.HasTenant)
        {
            return;
        }

        var tenantId = _tenantContext.TenantId!.Value;
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
