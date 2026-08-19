using iERP.SharedKernel.Exceptions;
using iERP.SharedKernel.Primitives;
using iERP.SharedKernel.Time;

namespace iERP.Modules.CRM.Domain;

/// <summary>
/// Opportunity aggregate. Created from a lead conversion; owns its follow-ups.
/// </summary>
public sealed class Opportunity : AuditableEntity
{
    private readonly List<OpportunityFollowUp> _followUps = [];

    private Opportunity()
    {
    }

    public Guid? SubsidiaryId { get; private set; }
    public string OpportunityNumber { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public Guid? CustomerId { get; private set; }
    public Guid? LeadId { get; private set; }
    public string Stage { get; private set; } = "prospecting";
    public decimal OpportunityValue { get; private set; }
    public string? CurrencyCode { get; private set; }
    public DateOnly? ExpectedCloseDate { get; private set; }
    public Guid? OwnerUserId { get; private set; }
    public string Status { get; private set; } = OpportunityStatuses.New;
    public string? StatusBeforeDiscard { get; private set; }
    public int Probability { get; private set; }
    public string? Computations { get; private set; }
    public string? Notes { get; private set; }
    public string? ClosedReason { get; private set; }

    public IReadOnlyCollection<OpportunityFollowUp> FollowUps => _followUps.AsReadOnly();

    public static Opportunity CreateFromLead(
        Guid tenantId,
        string opportunityNumber,
        Lead lead,
        decimal opportunityValue,
        int probability,
        string? status,
        string? computations,
        string? notes,
        string? closedReason,
        string? currencyCode,
        DateOnly? expectedCloseDate,
        Guid? ownerUserId)
    {
        var opportunity = new Opportunity();
        opportunity.SetTenantId(tenantId);
        opportunity.OpportunityNumber = opportunityNumber.Trim();
        opportunity.LeadId = lead.Id;
        opportunity.SubsidiaryId = lead.SubsidiaryId;
        opportunity.Name = string.IsNullOrWhiteSpace(lead.CompanyName)
            ? opportunityNumber.Trim()
            : lead.CompanyName.Trim();
        opportunity.OwnerUserId = ownerUserId ?? lead.AssignedToUserId;
        opportunity.ApplyDetails(
            opportunityValue,
            probability,
            string.IsNullOrWhiteSpace(status) ? OpportunityStatuses.New : status,
            computations,
            notes,
            closedReason,
            currencyCode,
            expectedCloseDate,
            opportunity.OwnerUserId);
        return opportunity;
    }

    public void Update(
        decimal opportunityValue,
        int probability,
        string? status,
        string? computations,
        string? notes,
        string? closedReason,
        string? currencyCode,
        DateOnly? expectedCloseDate,
        Guid? ownerUserId,
        string? name)
    {
        EnsureNotDeleted();
        if (Status.Equals(OpportunityStatuses.Discarded, StringComparison.OrdinalIgnoreCase))
        {
            throw new BusinessRuleException(
                ErrorCodes.BusinessRuleViolation,
                "Discarded opportunities cannot be updated. Restore first.");
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name.Trim();
        }

        ApplyDetails(
            opportunityValue,
            probability,
            string.IsNullOrWhiteSpace(status) ? Status : status,
            computations,
            notes,
            closedReason,
            currencyCode,
            expectedCloseDate,
            ownerUserId);
    }

    public void Discard()
    {
        EnsureNotDeleted();
        if (Status.Equals(OpportunityStatuses.Discarded, StringComparison.OrdinalIgnoreCase))
        {
            throw new BusinessRuleException(ErrorCodes.BusinessRuleViolation, "Opportunity is already discarded.");
        }

        StatusBeforeDiscard = Status;
        Status = OpportunityStatuses.Discarded;
    }

    public void Restore()
    {
        EnsureNotDeleted();
        if (!Status.Equals(OpportunityStatuses.Discarded, StringComparison.OrdinalIgnoreCase))
        {
            throw new BusinessRuleException(ErrorCodes.BusinessRuleViolation, "Only discarded opportunities can be restored.");
        }

        Status = string.IsNullOrWhiteSpace(StatusBeforeDiscard)
            ? OpportunityStatuses.New
            : StatusBeforeDiscard;
        StatusBeforeDiscard = null;
    }

    public void MarkDeleted(Guid? deletedBy, DateTimeOffset deletedAt)
    {
        SoftDelete(deletedBy, deletedAt);
    }

    public OpportunityFollowUp AddFollowUp(
        string activityType,
        DateTimeOffset followUpDate,
        DateTimeOffset? nextFollowUpDate,
        string? remarks,
        string? status)
    {
        EnsureNotDeleted();
        var followUp = OpportunityFollowUp.Create(
            TenantId,
            Id,
            activityType,
            followUpDate,
            nextFollowUpDate,
            remarks,
            status);
        _followUps.Add(followUp);
        return followUp;
    }

    private void ApplyDetails(
        decimal opportunityValue,
        int probability,
        string status,
        string? computations,
        string? notes,
        string? closedReason,
        string? currencyCode,
        DateOnly? expectedCloseDate,
        Guid? ownerUserId)
    {
        if (opportunityValue < 0)
        {
            throw new DomainException(ErrorCodes.ValidationError, "Opportunity value cannot be negative.");
        }

        if (probability is < 0 or > 100)
        {
            throw new DomainException(ErrorCodes.ValidationError, "Probability must be between 0 and 100.");
        }

        var normalizedStatus = status.Trim();
        if (!OpportunityStatuses.All.Contains(normalizedStatus))
        {
            throw new DomainException(ErrorCodes.ValidationError, $"Invalid opportunity status '{status}'.");
        }

        if (OpportunityStatuses.IsClosed(normalizedStatus) && string.IsNullOrWhiteSpace(closedReason))
        {
            throw new DomainException(
                ErrorCodes.ValidationError,
                "Closed reason is required when status is Won or Lost.");
        }

        OpportunityValue = opportunityValue;
        Probability = probability;
        Status = normalizedStatus;
        Computations = Normalize(computations);
        Notes = Normalize(notes);
        ClosedReason = OpportunityStatuses.IsClosed(normalizedStatus) ? Normalize(closedReason) : Normalize(closedReason);
        CurrencyCode = Normalize(currencyCode);
        ExpectedCloseDate = expectedCloseDate;
        OwnerUserId = ownerUserId;

        if (!OpportunityStatuses.IsClosed(normalizedStatus))
        {
            ClosedReason = string.IsNullOrWhiteSpace(closedReason) ? null : closedReason.Trim();
        }
    }

    private void EnsureNotDeleted()
    {
        if (IsDeleted)
        {
            throw new BusinessRuleException(ErrorCodes.BusinessRuleViolation, "Opportunity is deleted.");
        }
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
