using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Sales.Domain;

public sealed class SalesEnquiry : AuditableEntity
{
    private readonly List<SalesEnquiryLine> _lines = [];
    private readonly List<SalesEnquiryFollowUp> _followUps = [];

    public string EnquiryCode { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Opportunity { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? AltPhone { get; set; }
    public decimal? Probability { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateOnly? ExpectedClose { get; set; }
    public string Status { get; set; } = "Open";
    public string? WinLossReason { get; set; }
    public decimal? ProjectedTotal { get; set; }
    public string? ForecastType { get; set; }
    public decimal? WeightedTotal { get; set; }
    public string? Range { get; set; }
    public string? Subsidiary { get; set; }
    public string? ClassName { get; set; }
    public string? Location { get; set; }
    public string? Department { get; set; }
    public string? SalesRep { get; set; }
    public DateOnly? LastSalesActivity { get; set; }
    public string Currency { get; set; } = "INR - RUPEE";
    public decimal ExchangeRate { get; set; } = 1m;
    public string? Payment { get; set; }
    public string? Prices { get; set; }
    public string? Delivery { get; set; }

    public IReadOnlyCollection<SalesEnquiryLine> Lines => _lines.AsReadOnly();
    public IReadOnlyCollection<SalesEnquiryFollowUp> FollowUps => _followUps.AsReadOnly();

    public void ReplaceLines(IEnumerable<SalesEnquiryLine> lines)
    {
        _lines.Clear();
        _lines.AddRange(lines);
    }

    public void AddFollowUp(SalesEnquiryFollowUp followUp) => _followUps.Add(followUp);
}

public sealed class SalesEnquiryLine : AuditableEntity
{
    public Guid SalesEnquiryId { get; set; }
    public int LineNo { get; set; }
    public string? Category { get; set; }
    public string? Item { get; set; }
    public string? Description { get; set; }
    public decimal Quantity { get; set; }
    public string? Uom { get; set; }
    public string? PriceLevel { get; set; }
    public decimal Rate { get; set; }
    public decimal Discount { get; set; }
    public decimal Amount { get; set; }
    public string? TaxCode { get; set; }
    public decimal GrossAmount { get; set; }
    public string? ClassName { get; set; }
    public string? CountryOfOrigin { get; set; }
    public string? HsCode { get; set; }
}

public sealed class SalesEnquiryFollowUp : AuditableEntity
{
    public Guid SalesEnquiryId { get; set; }
    public DateOnly? FollowUpDate { get; set; }
    public DateOnly? NextFollowUpDate { get; set; }
    public string? Status { get; set; }
    public string? ActivityType { get; set; }
    public string? Remarks { get; set; }
}
