using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace iERP.Modules.Platform.Identity.Api;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth").WithTags("Auth");

        group.MapPost("/login", () =>
            Results.Json(ApiErrorResponse.Create("NOT_IMPLEMENTED", "Login endpoint is not implemented yet."), statusCode: StatusCodes.Status501NotImplemented));

        group.MapPost("/refresh", () =>
            Results.Json(ApiErrorResponse.Create("NOT_IMPLEMENTED", "Refresh endpoint is not implemented yet."), statusCode: StatusCodes.Status501NotImplemented));

        group.MapPost("/logout", () =>
            Results.Json(ApiErrorResponse.Create("NOT_IMPLEMENTED", "Logout endpoint is not implemented yet."), statusCode: StatusCodes.Status501NotImplemented));

        return app;
    }
}
