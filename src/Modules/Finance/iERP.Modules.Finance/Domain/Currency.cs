using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Finance.Domain;

public sealed class Currency : AuditableEntity
{

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int DecimalPlaces { get; set; } = 2;
    public bool IsActive { get; set; } = true;

}
