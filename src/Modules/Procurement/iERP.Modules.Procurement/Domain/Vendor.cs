using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Procurement.Domain;

public sealed class Vendor : AuditableEntity
{

    public Guid SubsidiaryId { get; set; }
    public string VendorCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? VendorCategory { get; set; }
    public string? CurrencyCode { get; set; }
    public Guid? PaymentTermsId { get; set; }
    public string? TaxRegistrationNumber { get; set; }
    public Guid? WithholdingTaxCodeId { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public string? BankSwiftCode { get; set; }
    public bool ApprovedVendor { get; set; }
    public string? CreditRating { get; set; }
    public string? Country { get; set; }
    public bool IsActive { get; set; } = true;

}
