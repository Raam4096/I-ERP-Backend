namespace iERP.SharedKernel.Results;

public sealed record ApiErrorResponse(
    bool Success,
    string Error,
    string Message,
    string? Field = null,
    IReadOnlyList<string>? Details = null)
{
    public static ApiErrorResponse Create(
        string error,
        string message,
        string? field = null,
        IReadOnlyList<string>? details = null) =>
        new(false, error, message, field, details);
}
