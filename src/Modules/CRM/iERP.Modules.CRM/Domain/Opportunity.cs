using iERP.SharedKernel.Primitives;

namespace iERP.Modules.CRM.Domain;

public sealed class Opportunity : AuditableEntity
{

    public Guid SubsidiaryId { get; set; }
    public string OpportunityNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? CustomerId { get; set; }
    public Guid? LeadId { get; set; }
    public string Stage { get; set; } = "prospecting";
    public decimal? Amount { get; set; }
    public string? CurrencyCode { get; set; }
    public DateOnly? ExpectedCloseDate { get; set; }
    public Guid? OwnerUserId { get; set; }
    public string Status { get; set; } = "open";

}
