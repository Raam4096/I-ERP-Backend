using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace iERP.Modules.Sales.Api;

public static class SalesExtraEndpoints
{
    public static IEndpointRouteBuilder MapSalesExtraEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/api/v1/sales_orders").WithTags("SalesOrders")
            .MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("Sales orders ready")));
        return app;
    }
}
