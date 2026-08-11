using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace iERP.Modules.Reporting.Api;

public static class ReportingEndpoints
{
    public static IEndpointRouteBuilder MapReportingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/reports").WithTags("Reporting");
        group.MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("Reporting module ready")))
            .WithName("ReportingHealth");
        return app;
    }
}
