using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.Settings.Domain;

public sealed class SystemSetting : AuditableEntity
{

    public Guid? SubsidiaryId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }

}
