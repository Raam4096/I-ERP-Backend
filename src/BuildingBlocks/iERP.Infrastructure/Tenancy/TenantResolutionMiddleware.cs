using iERP.SharedKernel.Tenancy;
using Microsoft.AspNetCore.Http;

namespace iERP.Infrastructure.Tenancy;

public sealed class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantResolver resolver, ITenantContext tenantContext)
    {
        var tenantId = await resolver.ResolveTenantIdAsync(context.RequestAborted);
        if (tenantId.HasValue)
        {
            tenantContext.SetTenant(tenantId.Value);
        }

        try
        {
            await _next(context);
        }
        finally
        {
            tenantContext.Clear();
        }
    }
}
