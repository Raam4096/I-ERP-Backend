using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Marine.Domain;

public sealed class Vessel : AuditableEntity
{

    public string VesselCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ImoNumber { get; set; }
    public string? VesselType { get; set; }
    public string? FlagState { get; set; }
    public decimal? GrossTonnage { get; set; }
    public int? YearBuilt { get; set; }
    public string? Owner { get; set; }
    public string? ClassificationSociety { get; set; }
    public DateOnly? ClassCertificateExpiry { get; set; }
    public Guid? CurrentPortLocationId { get; set; }
    public bool IsActive { get; set; } = true;

}
