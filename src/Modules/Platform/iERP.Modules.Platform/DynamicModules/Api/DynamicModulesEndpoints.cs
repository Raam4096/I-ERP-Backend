using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace iERP.Modules.Platform.DynamicModules.Api;

public static class DynamicModulesEndpoints
{
    public static IEndpointRouteBuilder MapDynamicModulesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/dynamic_modules").WithTags("DynamicModules");
        group.MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("DynamicModules module ready")))
            .WithName("DynamicModulesHealth");
        return app;
    }
}
