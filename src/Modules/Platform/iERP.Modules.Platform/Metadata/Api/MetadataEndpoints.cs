using iERP.Modules.Platform.Metadata.Application;
using iERP.Modules.Platform.Metadata.Application.Dtos;
using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace iERP.Modules.Platform.Metadata.Api;

public static class MetadataEndpoints
{
    public static IEndpointRouteBuilder MapMetadataEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/metadata")
            .WithTags("Metadata");

        group.MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("Metadata module ready")))
            .WithName("MetadataHealth")
            .AllowAnonymous();

        group.MapGet("/screens/{screenCode}", GetScreenAsync)
            .WithName("GetMetadataScreen")
            .RequireAuthorization()
            .Produces<ApiResponse<GenericPageDto>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound);

        return app;
    }

    private static async Task<IResult> GetScreenAsync(
        string screenCode,
        IMetadataScreenService screenService,
        CancellationToken cancellationToken)
    {
        var page = await screenService.GetScreenAsync(screenCode, cancellationToken);
        return Results.Ok(ApiResponse<GenericPageDto>.Ok(page));
    }
}
