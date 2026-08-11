using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Finance.Domain;

public sealed class ChartOfAccount : AuditableEntity
{

    public Guid SubsidiaryId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public Guid? ParentAccountId { get; set; }
    public bool IsPostable { get; set; } = true;
    public bool IsActive { get; set; } = true;

}
