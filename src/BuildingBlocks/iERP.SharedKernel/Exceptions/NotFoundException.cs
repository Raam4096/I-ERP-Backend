namespace iERP.SharedKernel.Exceptions;

public sealed class NotFoundException : DomainException
{
    public NotFoundException(string message)
        : base(ErrorCodes.NotFound, message)
    {
    }
}
