using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.Organization.Domain;

public sealed class Subsidiary : AuditableEntity
{

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public string? TaxRegistrationNumber { get; set; }
    public string? Country { get; set; }
    public string? CurrencyCode { get; set; }
    public bool IsActive { get; set; } = true;

}
