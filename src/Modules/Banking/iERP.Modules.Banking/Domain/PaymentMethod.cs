using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Banking.Domain;

public sealed class PaymentMethod : AuditableEntity
{

    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

}
