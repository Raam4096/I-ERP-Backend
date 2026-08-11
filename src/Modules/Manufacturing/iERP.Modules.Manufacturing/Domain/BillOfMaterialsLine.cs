using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Manufacturing.Domain;

public sealed class BillOfMaterialsLine : AuditableEntity
{

    public Guid BillOfMaterialsId { get; set; }
    public int LineNo { get; set; }
    public Guid ComponentItemId { get; set; }
    public decimal Quantity { get; set; }
    public Guid UomId { get; set; }

}
