using iERP.SharedKernel.Primitives;

namespace iERP.Modules.CRM.Domain;

public sealed class Address : AuditableEntity
{

    public Guid? CustomerId { get; set; }
    public Guid? VendorId { get; set; }
    public string AddressType { get; set; } = "billing";
    public string Line1 { get; set; } = string.Empty;
    public string? Line2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public bool IsDefault { get; set; }

}
