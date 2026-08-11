using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Projects.Domain;

public sealed class Project : AuditableEntity
{

    public Guid SubsidiaryId { get; set; }
    public Guid? BranchId { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? CustomerId { get; set; }
    public Guid? ContractId { get; set; }
    public string? ProjectType { get; set; }
    public Guid? ProjectManagerId { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public decimal? ContractValue { get; set; }
    public decimal? CostBudget { get; set; }
    public Guid? CostCenterId { get; set; }
    public string Status { get; set; } = "planned";

}
