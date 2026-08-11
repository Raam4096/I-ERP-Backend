using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.Identity.Domain;

public sealed class UserSubsidiary : AuditableEntity
{

    public Guid UserId { get; set; }
    public Guid SubsidiaryId { get; set; }
    public bool IsDefault { get; set; }

}
