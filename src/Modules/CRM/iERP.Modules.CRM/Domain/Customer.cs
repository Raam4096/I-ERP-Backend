using iERP.SharedKernel.Primitives;

namespace iERP.Modules.CRM.Domain;

public sealed class Customer : AuditableEntity
{

    public Guid SubsidiaryId { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string CustomerType { get; set; } = "company";
    public string? CurrencyCode { get; set; }
    public decimal? CreditLimit { get; set; }
    public Guid? PaymentTermsId { get; set; }
    public string? TaxRegistrationNumber { get; set; }
    public string? Country { get; set; }
    public Guid? DefaultPriceListId { get; set; }
    public Guid? SalespersonUserId { get; set; }
    public string? Industry { get; set; }
    public bool IsActive { get; set; } = true;

}
