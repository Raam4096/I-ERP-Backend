using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace iERP.Modules.CRM.Api;

public static class CrmEndpoints
{
    public static IEndpointRouteBuilder MapCrmEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/leads").WithTags("Crm");
        group.MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("Crm module ready")))
            .WithName("CrmHealth");
        return app;
    }
}
