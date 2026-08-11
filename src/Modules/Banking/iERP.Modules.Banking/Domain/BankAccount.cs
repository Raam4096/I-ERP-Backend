using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Banking.Domain;

public sealed class BankAccount : AuditableEntity
{

    public Guid SubsidiaryId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string? AccountNumber { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public Guid? GlAccountId { get; set; }
    public bool IsActive { get; set; } = true;

}
