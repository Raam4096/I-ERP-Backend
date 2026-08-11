using System.Security.Claims;
using iERP.SharedKernel.Tenancy;
using Microsoft.AspNetCore.Http;

namespace iERP.Infrastructure.Tenancy;

public sealed class ClaimTenantResolver : ITenantResolver
{
    public const string TenantIdClaimType = "tenant_id";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClaimTenantResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<Guid?> ResolveTenantIdAsync(CancellationToken cancellationToken = default)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return Task.FromResult<Guid?>(null);
        }

        var value = user.FindFirstValue(TenantIdClaimType) ?? user.FindFirstValue("tenantId");
        if (Guid.TryParse(value, out var tenantId))
        {
            return Task.FromResult<Guid?>(tenantId);
        }

        return Task.FromResult<Guid?>(null);
    }
}
