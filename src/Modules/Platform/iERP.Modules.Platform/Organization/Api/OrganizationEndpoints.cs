using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace iERP.Modules.Platform.Organization.Api;

public static class OrganizationEndpoints
{
    public static IEndpointRouteBuilder MapOrganizationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/organization").WithTags("Organization");
        group.MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("Organization module ready")))
            .WithName("OrganizationHealth");
        return app;
    }
}
