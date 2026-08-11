using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Manufacturing.Domain;

public sealed class WorkOrder : AuditableEntity
{

    public Guid SubsidiaryId { get; set; }
    public string DocumentNo { get; set; } = string.Empty;
    public Guid ItemId { get; set; }
    public Guid? BillOfMaterialsId { get; set; }
    public decimal Quantity { get; set; }
    public string Status { get; set; } = "planned";
    public DateOnly? PlannedStartDate { get; set; }
    public DateOnly? PlannedEndDate { get; set; }

}
