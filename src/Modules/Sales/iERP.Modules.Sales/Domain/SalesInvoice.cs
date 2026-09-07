using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Sales.Domain;

public sealed class SalesInvoice : AuditableEntity
{
    private readonly List<SalesInvoiceLine> _lines = [];

    public string InvoiceCode { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Title { get; set; }
    public string Status { get; set; } = "Draft";
    public DateOnly? DocumentDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public string? Subsidiary { get; set; }
    public string? ClassName { get; set; }
    public string? Location { get; set; }
    public string? Department { get; set; }
    public string? SalesRep { get; set; }
    public string Currency { get; set; } = "INR - RUPEE";
    public decimal ExchangeRate { get; set; } = 1m;
    public decimal? ProjectedTotal { get; set; }
    public string? Payment { get; set; }
    public string? Notes { get; set; }

    /// <summary>Optional link to a sales order (no FK — open for workflow changes).</summary>
    public Guid? SourceOrderId { get; set; }
    public string? SourceOrderCode { get; set; }

    /// <summary>Optional link to a sales quotation.</summary>
    public Guid? SourceQuotationId { get; set; }
    public string? SourceQuotationCode { get; set; }

    public Guid? SubsidiaryId { get; set; }
    public Guid? CustomerId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public IReadOnlyCollection<SalesInvoiceLine> Lines => _lines.AsReadOnly();

    public void ReplaceLines(IEnumerable<SalesInvoiceLine> lines)
    {
        _lines.Clear();
        _lines.AddRange(lines);
    }
}

public sealed class SalesInvoiceLine : AuditableEntity
{
    public Guid SalesInvoiceId { get; set; }
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

    public Guid? ItemId { get; set; }
    public Guid? UomId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public Guid? TaxCodeId { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineAmount { get; set; }
}
