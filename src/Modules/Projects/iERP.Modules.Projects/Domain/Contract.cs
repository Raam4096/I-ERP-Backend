using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Projects.Domain;

public sealed class Contract : AuditableEntity
{

    public Guid ProjectId { get; set; }
    public Guid CustomerId { get; set; }
    public string ContractNo { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ContractType { get; set; }
    public decimal ContractValue { get; set; }
    public decimal? RetentionPercent { get; set; }
    public decimal? RetentionCap { get; set; }
    public int? DefectsLiabilityMonths { get; set; }
    public string? BillingBasis { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

}
