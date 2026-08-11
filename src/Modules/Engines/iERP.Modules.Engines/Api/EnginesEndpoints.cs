using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace iERP.Modules.Engines.Api;

public static class EnginesEndpoints
{
    public static IEndpointRouteBuilder MapEnginesEndpoints(this IEndpointRouteBuilder app)
    {
        var workflows = app.MapGroup("/api/v1/workflows").WithTags("Workflows");
        workflows.MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("Workflow module ready")))
            .WithName("WorkflowHealth");

        var rules = app.MapGroup("/api/v1/rules").WithTags("Rules");
        rules.MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("Rules module ready")))
            .WithName("RulesHealth");

        var bridges = app.MapGroup("/api/v1/bridges").WithTags("Bridges");
        bridges.MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("Bridge module ready")))
            .WithName("BridgeHealth");

        var printing = app.MapGroup("/api/v1/printing").WithTags("Printing");
        printing.MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("Printing module ready")))
            .WithName("PrintingHealth");

        return app;
    }
}
