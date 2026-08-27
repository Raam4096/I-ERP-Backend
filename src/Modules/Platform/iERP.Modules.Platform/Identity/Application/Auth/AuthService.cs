using FluentValidation;
using iERP.Application.Abstractions.Options;
using iERP.Modules.Platform.Identity.Domain;
using iERP.Modules.Platform.Identity.Infrastructure;
using iERP.Modules.Platform.Tenancy.Infrastructure;
using iERP.SharedKernel.Exceptions;
using iERP.SharedKernel.Security;
using iERP.SharedKernel.Tenancy;
using iERP.SharedKernel.Time;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace iERP.Modules.Platform.Identity.Application.Auth;

public interface IAuthService
{
    Task<AuthTokenResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
    Task<AuthTokenResponse> RefreshAsync(RefreshRequest request, CancellationToken cancellationToken);
    Task LogoutAsync(LogoutRequest request, CancellationToken cancellationToken);
    Task<AuthUserDto> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken);
}

public sealed class AuthService : IAuthService
{
    private const string InvalidCredentialsMessage = "Invalid credentials.";

    private readonly PlatformDbContext _platformDb;
    private readonly IdentityDbContext _identityDb;
    private readonly ITenantContext _tenantContext;
    private readonly IPasswordHasher<AppUser> _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IClock _clock;
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        PlatformDbContext platformDb,
        IdentityDbContext identityDb,
        ITenantContext tenantContext,
        IPasswordHasher<AppUser> passwordHasher,
        IJwtTokenService jwtTokenService,
        IClock clock,
        IOptions<JwtOptions> jwtOptions,
        ILogger<AuthService> logger)
    {
        _platformDb = platformDb;
        _identityDb = identityDb;
        _tenantContext = tenantContext;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _clock = clock;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;
    }

    public async Task<AuthTokenResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var tenantCode = request.TenantCode.Trim();
        var email = request.Email.Trim().ToLowerInvariant();

        var tenant = await _platformDb.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => !x.IsDeleted && x.Code == tenantCode,
                cancellationToken);

        if (tenant is null || !string.Equals(tenant.Status, "active", StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedException(InvalidCredentialsMessage);
        }

        _tenantContext.SetTenant(tenant.Id);

        var user = await _identityDb.Users
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

        if (user is null || !user.IsActive || string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            throw new UnauthorizedException(InvalidCredentialsMessage);
        }

        var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verify == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedException(InvalidCredentialsMessage);
        }

        if (verify == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
        }

        user.LastLoginAt = _clock.UtcNow;
        var roles = await LoadRoleNamesAsync(user.Id, cancellationToken);
        var response = IssueTokens(user, tenant.Id, roles);
        await _identityDb.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {UserId} logged in for tenant {TenantId}", user.Id, tenant.Id);
        return response;
    }

    public async Task<AuthTokenResponse> RefreshAsync(RefreshRequest request, CancellationToken cancellationToken)
    {
        var tokenHash = _jwtTokenService.HashToken(request.RefreshToken);
        var now = _clock.UtcNow;

        var existing = await _identityDb.RefreshTokens
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash && !x.IsDeleted, cancellationToken);

        if (existing is null ||
            existing.RevokedAt.HasValue ||
            existing.ExpiresAt <= now)
        {
            throw new UnauthorizedException("Invalid or expired refresh token.", ErrorCodes.Unauthorized);
        }

        _tenantContext.SetTenant(existing.TenantId);

        var user = await _identityDb.Users
            .FirstOrDefaultAsync(x => x.Id == existing.UserId, cancellationToken);

        if (user is null || !user.IsActive)
        {
            existing.RevokedAt = now;
            await _identityDb.SaveChangesAsync(cancellationToken);
            throw new UnauthorizedException(InvalidCredentialsMessage);
        }

        var roles = await LoadRoleNamesAsync(user.Id, cancellationToken);
        var response = IssueTokens(user, existing.TenantId, roles);

        existing.RevokedAt = now;
        existing.ReplacedByTokenHash = _jwtTokenService.HashToken(response.RefreshToken);

        await _identityDb.SaveChangesAsync(cancellationToken);
        return response;
    }

    public async Task LogoutAsync(LogoutRequest request, CancellationToken cancellationToken)
    {
        var tokenHash = _jwtTokenService.HashToken(request.RefreshToken);
        var existing = await _identityDb.RefreshTokens
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash && !x.IsDeleted, cancellationToken);

        if (existing is null)
        {
            return;
        }

        _tenantContext.SetTenant(existing.TenantId);
        if (!existing.RevokedAt.HasValue)
        {
            existing.RevokedAt = _clock.UtcNow;
            await _identityDb.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<AuthUserDto> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        if (!_tenantContext.HasTenant)
        {
            throw new UnauthorizedException("Tenant context is required.");
        }

        var user = await _identityDb.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedException(InvalidCredentialsMessage);
        }

        var roles = await LoadRoleNamesAsync(user.Id, cancellationToken);
        return new AuthUserDto(
            user.Id,
            _tenantContext.TenantId!.Value,
            user.Email,
            user.UserName,
            user.DisplayName,
            roles);
    }

    private async Task<IReadOnlyList<string>> LoadRoleNamesAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await (
            from ur in _identityDb.UserRoles.AsNoTracking()
            join role in _identityDb.Roles.AsNoTracking() on ur.RoleId equals role.Id
            where ur.UserId == userId
            select role.Name).ToListAsync(cancellationToken);
    }

    private AuthTokenResponse IssueTokens(
        AppUser user,
        Guid tenantId,
        IReadOnlyList<string> roles)
    {
        var (accessToken, accessExpires) = _jwtTokenService.CreateAccessToken(user, tenantId, roles);
        var refreshToken = _jwtTokenService.CreateRefreshToken();
        var refreshExpires = _clock.UtcNow.AddDays(_jwtOptions.RefreshTokenDays);

        var refreshEntity = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = _jwtTokenService.HashToken(refreshToken),
            ExpiresAt = refreshExpires
        };
        refreshEntity.SetTenantId(tenantId);
        _identityDb.RefreshTokens.Add(refreshEntity);

        return new AuthTokenResponse(
            accessToken,
            refreshToken,
            accessExpires,
            refreshExpires,
            "Bearer",
            new AuthUserDto(user.Id, tenantId, user.Email, user.UserName, user.DisplayName, roles));
    }
}
