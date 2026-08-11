using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace iERP.Modules.HR.Api;

public static class HrEndpoints
{
    public static IEndpointRouteBuilder MapHrEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/hr").WithTags("Hr");
        group.MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("Hr module ready")))
            .WithName("HrHealth");
        return app;
    }
}
