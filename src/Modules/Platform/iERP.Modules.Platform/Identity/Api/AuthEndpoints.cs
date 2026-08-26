using FluentValidation;
using iERP.Modules.Platform.Identity.Application.Auth;
using iERP.SharedKernel.Exceptions;
using iERP.SharedKernel.Results;
using iERP.SharedKernel.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace iERP.Modules.Platform.Identity.Api;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var anonymous = app.MapGroup("/api/v1/auth")
            .WithTags("Auth")
            .AllowAnonymous();

        anonymous.MapPost("/login", LoginAsync)
            .WithName("Login")
            .Produces<ApiResponse<AuthTokenResponse>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status401Unauthorized);

        anonymous.MapPost("/refresh", RefreshAsync)
            .WithName("RefreshToken")
            .Produces<ApiResponse<AuthTokenResponse>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status401Unauthorized);

        anonymous.MapPost("/logout", LogoutAsync)
            .WithName("Logout")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest);

        var authorized = app.MapGroup("/api/v1/auth")
            .WithTags("Auth")
            .RequireAuthorization();

        authorized.MapGet("/me", MeAsync)
            .WithName("GetCurrentUser")
            .Produces<ApiResponse<AuthUserDto>>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status401Unauthorized);

        return app;
    }

    private static async Task<IResult> LoginAsync(
        [FromBody] LoginRequest request,
        IValidator<LoginRequest> validator,
        IAuthService authService,
        CancellationToken cancellationToken)
    {
        await ValidateAsync(validator, request, cancellationToken);
        var result = await authService.LoginAsync(request, cancellationToken);
        return Results.Ok(ApiResponse<AuthTokenResponse>.Ok(result, "Login successful."));
    }

    private static async Task<IResult> RefreshAsync(
        [FromBody] RefreshRequest request,
        IValidator<RefreshRequest> validator,
        IAuthService authService,
        CancellationToken cancellationToken)
    {
        await ValidateAsync(validator, request, cancellationToken);
        var result = await authService.RefreshAsync(request, cancellationToken);
        return Results.Ok(ApiResponse<AuthTokenResponse>.Ok(result, "Token refreshed."));
    }

    private static async Task<IResult> LogoutAsync(
        [FromBody] LogoutRequest request,
        IValidator<LogoutRequest> validator,
        IAuthService authService,
        CancellationToken cancellationToken)
    {
        await ValidateAsync(validator, request, cancellationToken);
        await authService.LogoutAsync(request, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> MeAsync(
        ICurrentUser currentUser,
        IAuthService authService,
        CancellationToken cancellationToken)
    {
        if (!currentUser.IsAuthenticated || currentUser.UserId is null)
        {
            throw new UnauthorizedException();
        }

        var user = await authService.GetCurrentUserAsync(currentUser.UserId.Value, cancellationToken);
        return Results.Ok(ApiResponse<AuthUserDto>.Ok(user));
    }

    private static async Task ValidateAsync<T>(
        IValidator<T> validator,
        T request,
        CancellationToken cancellationToken)
    {
        var result = await validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
        {
            throw new iERP.SharedKernel.Exceptions.ValidationException(
                "One or more validation errors occurred.",
                details: result.Errors.Select(e => e.ErrorMessage).ToArray());
        }
    }
}
