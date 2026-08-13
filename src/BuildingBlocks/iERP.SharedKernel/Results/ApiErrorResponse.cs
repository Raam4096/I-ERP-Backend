using System.Text.Json.Serialization;

namespace iERP.SharedKernel.Results;

public sealed record ApiErrorResponse(
    bool Success,
    string Error,
    string Message,
    string? Field = null,
    IReadOnlyList<string>? Errors = null)
{
    /// <summary>Backward-compatible alias for Errors. </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyList<string>? Details => Errors;

    public static ApiErrorResponse Create(
        string error,
        string message,
        string? field = null,
        IReadOnlyList<string>? errors = null) =>
        new(false, error, message, field, errors);
}
