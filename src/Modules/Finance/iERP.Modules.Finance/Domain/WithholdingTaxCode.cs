using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Finance.Domain;

public sealed class WithholdingTaxCode : AuditableEntity
{

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public bool IsActive { get; set; } = true;

}
