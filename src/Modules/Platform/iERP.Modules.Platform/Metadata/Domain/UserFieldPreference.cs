using iERP.SharedKernel.Primitives;

namespace iERP.Modules.Platform.Metadata.Domain;

/// <summary>
/// Per-user field visibility and display order for a screen (drag-drop / hide-unhide).
/// </summary>
public sealed class UserFieldPreference : AuditableEntity
{
    public Guid UserId { get; set; }
    public string ScreenCode { get; set; } = string.Empty;
    public string FieldKey { get; set; } = string.Empty;
    public bool IsVisible { get; set; } = true;
    public int DisplayOrder { get; set; }
}
