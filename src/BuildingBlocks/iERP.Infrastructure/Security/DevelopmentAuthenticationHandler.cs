using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace iERP.Infrastructure.Security;

/// <summary>
/// Development-only header auth. Requires an explicit X-Tenant-Id that matches a real tenant
/// (do not invent a fake GUID — that caused empty metadata/CRM results in Swagger).
/// Prefer JWT: Authorize in Swagger with the access token from POST /api/v1/auth/login.
/// </summary>
public sealed class DevelopmentAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "Development";
    public static readonly Guid DefaultUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    private readonly IWebHostEnvironment _environment;

    public DevelopmentAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IWebHostEnvironment environment)
        : base(options, logger, encoder)
    {
        _environment = environment;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!_environment.IsDevelopment())
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var tenantHeader = Request.Headers["X-Tenant-Id"].FirstOrDefault();
        if (!Guid.TryParse(tenantHeader, out var tenantId) || tenantId == Guid.Empty)
        {
            // No fake default tenant — forces JWT Authorize in Swagger / explicit header.
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var userHeader = Request.Headers["X-User-Id"].FirstOrDefault();
        var userId = Guid.TryParse(userHeader, out var u) && u != Guid.Empty ? u : DefaultUserId;

        var claims = new List<Claim>
        {
            new("tenant_id", tenantId.ToString()),
            new("user_id", userId.ToString()),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, "dev-user")
        };

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
