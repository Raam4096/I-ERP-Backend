using iERP.SharedKernel.Primitives;

namespace iERP.Modules.CRM.Domain;

public sealed class Contact : AuditableEntity
{

    public Guid? CustomerId { get; set; }
    public Guid? VendorId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? JobTitle { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; } = true;

}
