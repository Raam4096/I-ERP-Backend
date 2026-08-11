namespace iERP.SharedKernel.Security;

/// <summary>
/// Describes field-level permission for an entity field within a tenant/role context.
/// </summary>
public sealed class FieldPermission
{
    public required string EntityName { get; init; }
    public required string FieldKey { get; init; }
    public bool CanView { get; init; } = true;
    public bool CanEdit { get; init; }
}
