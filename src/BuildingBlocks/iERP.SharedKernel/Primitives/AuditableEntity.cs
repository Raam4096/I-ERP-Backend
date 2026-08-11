namespace iERP.SharedKernel.Primitives;

public abstract class AuditableEntity : TenantEntity, IAuditable, ISoftDeletable
{
    public DateTimeOffset CreatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
    public long Version { get; set; }

    protected AuditableEntity()
    {
    }

    protected AuditableEntity(Guid tenantId) : base(tenantId)
    {
    }

    public void SoftDelete(Guid? deletedBy, DateTimeOffset deletedAt)
    {
        IsDeleted = true;
        DeletedBy = deletedBy;
        DeletedAt = deletedAt;
    }
}
