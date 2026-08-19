using iERP.SharedKernel.Exceptions;
using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace iERP.Infrastructure.Exceptions;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, error) = Map(exception);
        if (status >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception");
        }
        else
        {
            _logger.LogWarning(exception, "Handled domain exception {ErrorCode}", error.Error);
        }

        httpContext.Response.StatusCode = status;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsJsonAsync(error, cancellationToken);
        return true;
    }

    private static (int Status, ApiErrorResponse Error) Map(Exception exception) =>
        exception switch
        {
            ValidationException vex => (
                StatusCodes.Status400BadRequest,
                ApiErrorResponse.Create(vex.ErrorCode, vex.Message, vex.Field, vex.Details)),
            NotFoundException nex => (
                StatusCodes.Status404NotFound,
                ApiErrorResponse.Create(nex.ErrorCode, nex.Message)),
            ForbiddenException fex => (
                StatusCodes.Status403Forbidden,
                ApiErrorResponse.Create(fex.ErrorCode, fex.Message)),
            UnauthorizedException uex => (
                StatusCodes.Status401Unauthorized,
                ApiErrorResponse.Create(uex.ErrorCode, uex.Message)),
            BusinessRuleException brex => (
                StatusCodes.Status409Conflict,
                ApiErrorResponse.Create(brex.ErrorCode, brex.Message)),
            DomainException dex => (
                StatusCodes.Status400BadRequest,
                ApiErrorResponse.Create(dex.ErrorCode, dex.Message)),
            _ => (
                StatusCodes.Status500InternalServerError,
                ApiErrorResponse.Create(ErrorCodes.InternalError, "An unexpected error occurred."))
        };
}
