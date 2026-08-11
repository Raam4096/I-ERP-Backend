namespace iERP.SharedKernel.Primitives;

public abstract class TenantEntity : Entity, ITenantEntity
{
    public Guid TenantId { get; protected set; }

    protected TenantEntity()
    {
    }

    protected TenantEntity(Guid tenantId)
    {
        TenantId = tenantId;
    }

    public void SetTenantId(Guid tenantId)
    {
        if (TenantId != Guid.Empty && TenantId != tenantId)
        {
            throw new InvalidOperationException("TenantId cannot be changed after it has been assigned.");
        }

        TenantId = tenantId;
    }
}
