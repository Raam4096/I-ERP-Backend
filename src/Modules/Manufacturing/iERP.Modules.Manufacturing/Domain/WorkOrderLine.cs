using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Manufacturing.Domain;

public sealed class WorkOrderLine : AuditableEntity
{

    public Guid WorkOrderId { get; set; }
    public int LineNo { get; set; }
    public Guid ComponentItemId { get; set; }
    public decimal RequiredQuantity { get; set; }
    public decimal IssuedQuantity { get; set; }

}
