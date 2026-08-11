namespace iERP.SharedKernel.Exceptions;

public sealed class ForbiddenException : DomainException
{
    public ForbiddenException(string message, string errorCode = ErrorCodes.Forbidden)
        : base(errorCode, message)
    {
    }
}
