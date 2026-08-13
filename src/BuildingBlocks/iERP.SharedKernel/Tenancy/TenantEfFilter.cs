namespace iERP.SharedKernel.Tenancy;

/// <summary>
/// Async-local tenant id used by EF Core global query filters.
/// Must be set from the same request flow as <see cref="ITenantContext"/>.
/// </summary>
public static class TenantEfFilter
{
    private static readonly AsyncLocal<Guid?> Current = new();

    public static Guid? TenantId
    {
        get => Current.Value;
        set => Current.Value = value;
    }

    public static void Clear() => Current.Value = null;
}
