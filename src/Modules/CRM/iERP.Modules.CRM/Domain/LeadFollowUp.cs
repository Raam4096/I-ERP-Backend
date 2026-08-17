using iERP.SharedKernel.Primitives;
using iERP.SharedKernel.Time;

namespace iERP.Modules.CRM.Domain;

public sealed class LeadFollowUp : AuditableEntity
{
    private readonly List<LeadAttachment> _attachments = [];

    private LeadFollowUp()
    {
    }

    public Guid LeadId { get; private set; }
    public Lead? Lead { get; private set; }
    public string ActivityType { get; private set; } = string.Empty;
    public DateTimeOffset FollowUpDate { get; private set; }
    public DateTimeOffset? NextFollowUpDate { get; private set; }
    public string? Remarks { get; private set; }
    public string Status { get; private set; } = FollowUpStatuses.Open;

    public IReadOnlyCollection<LeadAttachment> Attachments => _attachments.AsReadOnly();

    public static LeadFollowUp Create(
        Guid tenantId,
        Guid leadId,
        string activityType,
        DateTimeOffset followUpDate,
        DateTimeOffset? nextFollowUpDate,
        string? remarks,
        string? status)
    {
        var entity = new LeadFollowUp();
        entity.SetTenantId(tenantId);
        entity.LeadId = leadId;
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

    public LeadAttachment AddAttachment(
        string fileName,
        string filePath,
        string contentType,
        long fileSize,
        DateTimeOffset createdAt)
    {
        var attachment = LeadAttachment.Create(Id, fileName, filePath, contentType, fileSize, createdAt);
        _attachments.Add(attachment);
        return attachment;
    }

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
