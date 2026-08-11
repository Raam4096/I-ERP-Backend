using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Sales.Domain;

public sealed class SalesQuotation : AuditableEntity
{

    public Guid SubsidiaryId { get; set; }
    public string DocumentNo { get; set; } = string.Empty;
    public DateOnly DocumentDate { get; set; }
    public Guid CustomerId { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public decimal ExchangeRate { get; set; } = 1m;
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "draft";
    public string? Notes { get; set; }


}
