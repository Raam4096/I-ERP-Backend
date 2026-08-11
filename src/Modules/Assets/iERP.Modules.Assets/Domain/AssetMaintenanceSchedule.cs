using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Assets.Domain;

public sealed class AssetMaintenanceSchedule : AuditableEntity
{

    public Guid AssetId { get; set; }
    public string ScheduleName { get; set; } = string.Empty;
    public string Frequency { get; set; } = "monthly";
    public DateOnly? NextDueDate { get; set; }
    public bool IsActive { get; set; } = true;

}
