using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace iERP.Modules.Marine.Api;

public static class MarineEndpoints
{
    public static IEndpointRouteBuilder MapMarineEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/marine").WithTags("Marine");
        group.MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("Marine module ready")))
            .WithName("MarineHealth");
        return app;
    }
}
