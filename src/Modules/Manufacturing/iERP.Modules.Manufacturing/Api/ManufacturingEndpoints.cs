using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace iERP.Modules.Manufacturing.Api;

public static class ManufacturingEndpoints
{
    public static IEndpointRouteBuilder MapManufacturingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/manufacturing").WithTags("Manufacturing");
        group.MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("Manufacturing module ready")))
            .WithName("ManufacturingHealth");
        return app;
    }
}
