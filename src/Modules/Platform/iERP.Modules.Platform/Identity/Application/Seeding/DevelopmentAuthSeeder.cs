using iERP.Application.Abstractions.Options;
using iERP.Modules.Platform.Identity.Domain;
using iERP.Modules.Platform.Identity.Infrastructure;
using iERP.Modules.Platform.Tenancy.Domain;
using iERP.Modules.Platform.Tenancy.Infrastructure;
using iERP.SharedKernel.Security;
using iERP.SharedKernel.Tenancy;
using iERP.SharedKernel.Time;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace iERP.Modules.Platform.Identity.Application.Seeding;

/// <summary>
/// Creates a demo tenant + admin user when AuthSeed:Enabled is true (intended for Development only).
/// </summary>
public sealed class DevelopmentAuthSeeder
{
    private readonly PlatformDbContext _platformDb;
    private readonly IdentityDbContext _identityDb;
    private readonly ITenantContext _tenantContext;
    private readonly IPasswordHasher<AppUser> _passwordHasher;
    private readonly IClock _clock;
    private readonly AuthSeedOptions _options;
    private readonly ILogger<DevelopmentAuthSeeder> _logger;

    public DevelopmentAuthSeeder(
        PlatformDbContext platformDb,
        IdentityDbContext identityDb,
        ITenantContext tenantContext,
        IPasswordHasher<AppUser> passwordHasher,
        IClock clock,
        IOptions<AuthSeedOptions> options,
        ILogger<DevelopmentAuthSeeder> logger)
    {
        _platformDb = platformDb;
        _identityDb = identityDb;
        _tenantContext = tenantContext;
        _passwordHasher = passwordHasher;
        _clock = clock;
        _options = options.Value;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_options.AdminPassword) || _options.AdminPassword.Length < 8)
        {
            _logger.LogWarning("AuthSeed is enabled but AdminPassword is missing or too short; skipping seed.");
            return;
        }

        var tenantCode = _options.TenantCode.Trim();
        var tenant = await _platformDb.Tenants
            .FirstOrDefaultAsync(x => x.Code == tenantCode && !x.IsDeleted, cancellationToken);

        if (tenant is null)
        {
            tenant = new Tenant
            {
                Code = tenantCode,
                Name = string.IsNullOrWhiteSpace(_options.TenantName) ? tenantCode : _options.TenantName.Trim(),
                Status = "active",
                CreatedAt = _clock.UtcNow
            };
            _platformDb.Tenants.Add(tenant);
            await _platformDb.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Seeded tenant {TenantCode} ({TenantId})", tenant.Code, tenant.Id);
        }

        _tenantContext.SetTenant(tenant.Id);

        var email = _options.AdminEmail.Trim().ToLowerInvariant();
        var user = await _identityDb.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        if (user is null)
        {
            user = new AppUser
            {
                Email = email,
                UserName = string.IsNullOrWhiteSpace(_options.AdminUserName) ? email : _options.AdminUserName.Trim(),
                DisplayName = _options.AdminDisplayName,
                IsActive = true
            };
            user.SetTenantId(tenant.Id);
            user.PasswordHash = _passwordHasher.HashPassword(user, _options.AdminPassword);
            _identityDb.Users.Add(user);
            await _identityDb.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Seeded admin user {Email} for tenant {TenantCode}", email, tenantCode);
        }

        var role = await _identityDb.Roles
            .FirstOrDefaultAsync(x => x.Name == SystemRoles.TenantAdmin, cancellationToken);
        if (role is null)
        {
            role = new AppRole
            {
                Name = SystemRoles.TenantAdmin,
                Description = "Tenant administrator",
                IsSystemRole = true
            };
            role.SetTenantId(tenant.Id);
            _identityDb.Roles.Add(role);
            await _identityDb.SaveChangesAsync(cancellationToken);
        }

        var hasRole = await _identityDb.UserRoles
            .AnyAsync(x => x.UserId == user.Id && x.RoleId == role.Id, cancellationToken);
        if (!hasRole)
        {
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            };
            userRole.SetTenantId(tenant.Id);
            _identityDb.UserRoles.Add(userRole);
            await _identityDb.SaveChangesAsync(cancellationToken);
        }
    }
}
