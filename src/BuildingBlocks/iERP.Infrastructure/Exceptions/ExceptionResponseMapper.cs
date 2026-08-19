using iERP.SharedKernel.Exceptions;
using iERP.SharedKernel.Results;
using Microsoft.AspNetCore.Http;

namespace iERP.Infrastructure.Exceptions;

/// <summary>
/// Maps domain/application exceptions to HTTP status + <see cref="ApiErrorResponse"/>.
/// Kept separate from the ASP.NET handler so mapping rules are unit-testable (SRP).
/// </summary>
public static class ExceptionResponseMapper
{
    public static (int StatusCode, ApiErrorResponse Error) Map(Exception exception) =>
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
