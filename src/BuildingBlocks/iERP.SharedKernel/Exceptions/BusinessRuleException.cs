namespace iERP.SharedKernel.Exceptions;

public sealed class BusinessRuleException : DomainException
{
    public BusinessRuleException(string errorCode, string message)
        : base(errorCode, message)
    {
    }
}
