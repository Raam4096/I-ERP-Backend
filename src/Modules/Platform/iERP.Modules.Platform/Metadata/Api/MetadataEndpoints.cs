using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace iERP.Modules.Platform.Metadata.Api;

public static class MetadataEndpoints
{
    public static IEndpointRouteBuilder MapMetadataEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/metadata").WithTags("Metadata");
        group.MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("Metadata module ready")))
            .WithName("MetadataHealth");
        return app;
    }
}
