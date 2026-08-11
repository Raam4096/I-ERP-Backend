using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Assets.Domain;

public sealed class Asset : AuditableEntity
{

    public Guid SubsidiaryId { get; set; }
    public string AssetCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid? AssetTypeId { get; set; }
    public string? SerialNumber { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public DateOnly? PurchaseDate { get; set; }
    public decimal? PurchasePrice { get; set; }
    public string? DepreciationMethod { get; set; }
    public int? UsefulLifeYears { get; set; }
    public string? CurrentLocation { get; set; }
    public DateOnly? MaintenanceDueDate { get; set; }
    public DateOnly? CertificateExpiry { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? GlAssetAccountId { get; set; }
    public bool IsActive { get; set; } = true;

}
