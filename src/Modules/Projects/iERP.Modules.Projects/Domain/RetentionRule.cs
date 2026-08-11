using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Projects.Domain;

public sealed class RetentionRule : AuditableEntity
{

    public Guid ContractId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Percent { get; set; }
    public decimal? CapAmount { get; set; }

}
