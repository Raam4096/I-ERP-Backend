using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace iERP.Modules.Projects.Api;

public static class ProjectsEndpoints
{
    public static IEndpointRouteBuilder MapProjectsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/projects").WithTags("Projects");
        group.MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("Projects module ready")))
            .WithName("ProjectsHealth");
        return app;
    }
}
