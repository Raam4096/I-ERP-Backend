using iERP.SharedKernel.Primitives;

namespace iERP.Modules.CRM.Domain;

public sealed class Activity : AuditableEntity
{

    public Guid SubsidiaryId { get; set; }
    public string ActivityType { get; set; } = "call";
    public string Subject { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public Guid? LeadId { get; set; }
    public Guid? OpportunityId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? OwnerUserId { get; set; }
    public DateTimeOffset? DueAt { get; set; }
    public string Status { get; set; } = "planned";

}
