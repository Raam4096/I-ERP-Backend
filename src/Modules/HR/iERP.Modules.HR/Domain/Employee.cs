using iERP.SharedKernel.Primitives;

namespace iERP.Modules.HR.Domain;

public sealed class Employee : AuditableEntity
{

    public Guid SubsidiaryId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? IdentificationNumber { get; set; }
    public string? Nationality { get; set; }
    public string? Designation { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? EmploymentType { get; set; }
    public DateOnly? JoinDate { get; set; }
    public string? SalaryGrade { get; set; }
    public decimal? AnnualLeaveEntitlement { get; set; }
    public string? WorkPassNumber { get; set; }
    public DateOnly? WorkPassExpiry { get; set; }
    public Guid? LinkedUserId { get; set; }
    public bool IsActive { get; set; } = true;

}
