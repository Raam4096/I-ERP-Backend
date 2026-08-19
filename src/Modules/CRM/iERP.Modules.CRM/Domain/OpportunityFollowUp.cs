using iERP.SharedKernel.Primitives;
using iERP.SharedKernel.Time;

namespace iERP.Modules.CRM.Domain;

public sealed class OpportunityFollowUp : AuditableEntity
{
    private OpportunityFollowUp()
    {
    }

    public Guid OpportunityId { get; private set; }
    public Opportunity? Opportunity { get; private set; }
    public string ActivityType { get; private set; } = string.Empty;
    public DateTimeOffset FollowUpDate { get; private set; }
    public DateTimeOffset? NextFollowUpDate { get; private set; }
    public string? Remarks { get; private set; }
    public string Status { get; private set; } = FollowUpStatuses.Open;

    public static OpportunityFollowUp Create(
        Guid tenantId,
        Guid opportunityId,
        string activityType,
        DateTimeOffset followUpDate,
        DateTimeOffset? nextFollowUpDate,
        string? remarks,
        string? status)
    {
        var entity = new OpportunityFollowUp();
        entity.SetTenantId(tenantId);
        entity.OpportunityId = opportunityId;
        entity.Apply(activityType, followUpDate, nextFollowUpDate, remarks, status);
        return entity;
    }

    public void Update(
        string activityType,
        DateTimeOffset followUpDate,
        DateTimeOffset? nextFollowUpDate,
        string? remarks,
        string? status) =>
        Apply(activityType, followUpDate, nextFollowUpDate, remarks, status);

    private void Apply(
        string activityType,
        DateTimeOffset followUpDate,
        DateTimeOffset? nextFollowUpDate,
        string? remarks,
        string? status)
    {
        ActivityType = activityType.Trim();
        FollowUpDate = DateTimeOffsetUtc.Normalize(followUpDate);
        NextFollowUpDate = DateTimeOffsetUtc.Normalize(nextFollowUpDate);
        Remarks = string.IsNullOrWhiteSpace(remarks) ? null : remarks.Trim();
        Status = string.IsNullOrWhiteSpace(status) ? FollowUpStatuses.Open : status.Trim();
    }
}
