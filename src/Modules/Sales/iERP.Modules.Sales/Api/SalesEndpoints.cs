using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace iERP.Modules.Sales.Api;

public static class SalesEndpoints
{
    public static IEndpointRouteBuilder MapSalesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/sales_quotations").WithTags("Sales");
        group.MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("Sales module ready")))
            .WithName("SalesHealth");
        return app;
    }
}
