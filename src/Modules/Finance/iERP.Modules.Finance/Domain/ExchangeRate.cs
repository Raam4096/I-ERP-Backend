using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Finance.Domain;

public sealed class ExchangeRate : AuditableEntity
{

    public string FromCurrencyCode { get; set; } = string.Empty;
    public string ToCurrencyCode { get; set; } = string.Empty;
    public DateOnly RateDate { get; set; }
    public decimal Rate { get; set; }

}
