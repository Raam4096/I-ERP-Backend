using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace iERP.Modules.Banking.Api;

public static class BankingEndpoints
{
    public static IEndpointRouteBuilder MapBankingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/banking").WithTags("Banking");
        group.MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("Banking module ready")))
            .WithName("BankingHealth");
        return app;
    }
}
