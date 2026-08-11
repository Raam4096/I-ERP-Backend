namespace iERP.SharedKernel.Exceptions;

public sealed class ValidationException : DomainException
{
    public string? Field { get; }
    public IReadOnlyList<string> Details { get; }

    public ValidationException(string message, string? field = null, IEnumerable<string>? details = null)
        : base(ErrorCodes.ValidationError, message)
    {
        Field = field;
        Details = details?.ToArray() ?? [];
    }
}
