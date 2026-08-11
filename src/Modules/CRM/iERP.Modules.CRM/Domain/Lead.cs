using iERP.SharedKernel.Primitives;

namespace iERP.Modules.CRM.Domain;

public sealed class Lead : AuditableEntity
{

    public Guid SubsidiaryId { get; set; }
    public string LeadNumber { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? CompanyName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Source { get; set; }
    public string Status { get; set; } = "new";
    public string? Rating { get; set; }
    public decimal? EstimatedValue { get; set; }
    public string? CurrencyCode { get; set; }
    public Guid? OwnerUserId { get; set; }
    public Guid? ConvertedCustomerId { get; set; }
    public Guid? ConvertedContactId { get; set; }
    public Guid? ConvertedOpportunityId { get; set; }
    public DateTimeOffset? ConvertedAt { get; set; }

}
