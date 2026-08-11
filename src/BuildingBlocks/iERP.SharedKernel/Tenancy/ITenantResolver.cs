namespace iERP.SharedKernel.Tenancy;

public interface ITenantResolver
{
    Task<Guid?> ResolveTenantIdAsync(CancellationToken cancellationToken = default);
}
