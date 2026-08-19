namespace iERP.SharedKernel.Exceptions;

public sealed class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message = "Invalid credentials.", string errorCode = ErrorCodes.Unauthorized)
        : base(errorCode, message)
    {
    }
}
