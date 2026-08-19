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
        var (status, error) = ExceptionResponseMapper.Map(exception);
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
}
