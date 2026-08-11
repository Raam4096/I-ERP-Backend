using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace iERP.Modules.Procurement.Api;

public static class ProcurementEndpoints
{
    public static IEndpointRouteBuilder MapProcurementEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/purchase_orders").WithTags("Procurement");
        group.MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("Procurement module ready")))
            .WithName("ProcurementHealth");
        return app;
    }
}
