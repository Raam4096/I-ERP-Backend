namespace iERP.Modules.Sales.Application.Dtos;

public sealed record SalesLineItemDto(
    string? Category,
    string? Item,
    string? Description,
    decimal Quantity,
    string? Uom,
    string? PriceLevel,
    decimal Rate,
    decimal Discount,
    decimal Amount,
    string? TaxCode,
    decimal GrossAmount,
    string? ClassName,
    string? CountryOfOrigin,
    string? HsCode);

public sealed record SalesFollowUpDto(
    Guid? Id,
    DateOnly? FollowUpDate,
    DateOnly? NextFollowUpDate,
    string? Status,
    string? ActivityType,
    string? Remarks);

public sealed class SalesEnquiryDto
{
    public Guid Id { get; init; }
    public string EnquiryCode { get; init; } = string.Empty;
    public string Customer { get; init; } = string.Empty;
    public string? ContactPerson { get; init; }
    public string? Opportunity { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? AltPhone { get; init; }
    public decimal? Probability { get; init; }
    public string Title { get; init; } = string.Empty;
    public DateOnly? ExpectedClose { get; init; }
    public string Status { get; init; } = "Open";
    public string? WinLossReason { get; init; }
    public decimal? ProjectedTotal { get; init; }
    public string? ForecastType { get; init; }
    public decimal? WeightedTotal { get; init; }
    public string? Range { get; init; }
    public string? Subsidiary { get; init; }
    public string? ClassName { get; init; }
    public string? Location { get; init; }
    public string? Department { get; init; }
    public string? SalesRep { get; init; }
    public DateOnly? LastSalesActivity { get; init; }
    public string Currency { get; init; } = "INR - RUPEE";
    public decimal ExchangeRate { get; init; } = 1m;
    public string? Payment { get; init; }
    public string? Prices { get; init; }
    public string? Delivery { get; init; }
    public bool CreateInitialFollowUp { get; init; }
    public DateOnly? FollowUpDate { get; init; }
    public DateOnly? NextFollowUpDate { get; init; }
    public string? FollowUpStatus { get; init; }
    public string? ActivityType { get; init; }
    public string? Remarks { get; init; }
    public IReadOnlyList<SalesLineItemDto> Items { get; init; } = [];
    public IReadOnlyList<SalesFollowUpDto> FollowUps { get; init; } = [];
}

public sealed class UpsertSalesEnquiryRequest
{
    public string? EnquiryCode { get; set; }
    public string Customer { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Opportunity { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? AltPhone { get; set; }
    public decimal? Probability { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateOnly? ExpectedClose { get; set; }
    public string? Status { get; set; }
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
    public string? Currency { get; set; }
    public decimal? ExchangeRate { get; set; }
    public string? Payment { get; set; }
    public string? Prices { get; set; }
    public string? Delivery { get; set; }
    public bool CreateInitialFollowUp { get; set; }
    public DateOnly? FollowUpDate { get; set; }
    public DateOnly? NextFollowUpDate { get; set; }
    public string? FollowUpStatus { get; set; }
    public string? ActivityType { get; set; }
    public string? Remarks { get; set; }
    public IReadOnlyList<SalesLineItemDto>? Items { get; set; }
}

public sealed class SalesQuotationDto
{
    public Guid Id { get; init; }
    public string QuotationCode { get; init; } = string.Empty;
    public string Customer { get; init; } = string.Empty;
    public string? ContactPerson { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Status { get; init; } = "Draft";
    public DateOnly? DocumentDate { get; init; }
    public DateOnly? ValidUntil { get; init; }
    public string? Subsidiary { get; init; }
    public string? ClassName { get; init; }
    public string? Location { get; init; }
    public string? Department { get; init; }
    public string? SalesRep { get; init; }
    public string Currency { get; init; } = "INR - RUPEE";
    public decimal ExchangeRate { get; init; } = 1m;
    public decimal? ProjectedTotal { get; init; }
    public string? Payment { get; init; }
    public string? Prices { get; init; }
    public string? Delivery { get; init; }
    public string? Notes { get; init; }
    public IReadOnlyList<SalesLineItemDto> Items { get; init; } = [];
}

public sealed class UpsertSalesQuotationRequest
{
    public string? QuotationCode { get; set; }
    public string Customer { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Status { get; set; }
    public DateOnly? DocumentDate { get; set; }
    public DateOnly? ValidUntil { get; set; }
    public string? Subsidiary { get; set; }
    public string? ClassName { get; set; }
    public string? Location { get; set; }
    public string? Department { get; set; }
    public string? SalesRep { get; set; }
    public string? Currency { get; set; }
    public decimal? ExchangeRate { get; set; }
    public decimal? ProjectedTotal { get; set; }
    public string? Payment { get; set; }
    public string? Prices { get; set; }
    public string? Delivery { get; set; }
    public string? Notes { get; set; }
    public IReadOnlyList<SalesLineItemDto>? Items { get; set; }
}

public sealed class SalesInvoiceDto
{
    public Guid Id { get; init; }
    public string InvoiceCode { get; init; } = string.Empty;
    public string Customer { get; init; } = string.Empty;
    public string? ContactPerson { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Title { get; init; }
    public string Status { get; init; } = "Draft";
    public DateOnly? DocumentDate { get; init; }
    public DateOnly? DueDate { get; init; }
    public string? Subsidiary { get; init; }
    public string? ClassName { get; init; }
    public string? Location { get; init; }
    public string? Department { get; init; }
    public string? SalesRep { get; init; }
    public string Currency { get; init; } = "INR - RUPEE";
    public decimal ExchangeRate { get; init; } = 1m;
    public decimal? ProjectedTotal { get; init; }
    public string? Payment { get; init; }
    public string? Notes { get; init; }
    public IReadOnlyList<SalesLineItemDto> Items { get; init; } = [];
}

public sealed class UpsertSalesInvoiceRequest
{
    public string? InvoiceCode { get; set; }
    public string Customer { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Title { get; set; }
    public string? Status { get; set; }
    public DateOnly? DocumentDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public string? Subsidiary { get; set; }
    public string? ClassName { get; set; }
    public string? Location { get; set; }
    public string? Department { get; set; }
    public string? SalesRep { get; set; }
    public string? Currency { get; set; }
    public decimal? ExchangeRate { get; set; }
    public decimal? ProjectedTotal { get; set; }
    public string? Payment { get; set; }
    public string? Notes { get; set; }
    public IReadOnlyList<SalesLineItemDto>? Items { get; set; }
}
