using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace iERP.Modules.Platform.Attachments.Api;

public static class AttachmentsEndpoints
{
    public static IEndpointRouteBuilder MapAttachmentsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/attachments").WithTags("Attachments");
        group.MapGet("/health", () => Results.Ok(ApiResponse<string>.Ok("Attachments module ready")))
            .WithName("AttachmentsHealth");
        return app;
    }
}
