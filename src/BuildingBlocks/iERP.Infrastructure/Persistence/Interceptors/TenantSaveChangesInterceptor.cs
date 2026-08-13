using iERP.SharedKernel.Primitives;
using iERP.SharedKernel.Security;
using iERP.SharedKernel.Tenancy;
using iERP.SharedKernel.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace iERP.Infrastructure.Persistence.Interceptors;

public sealed class TenantSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ITenantContext _tenantContext;
    private readonly IClock _clock;
    private readonly ICurrentUser _currentUser;

    public TenantSaveChangesInterceptor(
        ITenantContext tenantContext,
        IClock clock,
        ICurrentUser currentUser)
    {
        _tenantContext = tenantContext;
        _clock = clock;
        _currentUser = currentUser;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ApplyRules(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ApplyRules(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void ApplyRules(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        foreach (var entry in context.ChangeTracker.Entries<ITenantEntity>())
        {
            if (entry.State is EntityState.Added)
            {
                if (_tenantContext.HasTenant)
                {
                    if (entry.Entity is TenantEntity tenantEntity && tenantEntity.TenantId == Guid.Empty)
                    {
                        tenantEntity.SetTenantId(_tenantContext.TenantId!.Value);
                    }
                    else if (entry.Entity.TenantId != _tenantContext.TenantId)
                    {
                        throw new InvalidOperationException(
                            $"Entity tenant '{entry.Entity.TenantId}' does not match current tenant '{_tenantContext.TenantId}'.");
                    }
                }
            }

            if (entry.State is EntityState.Modified)
            {
                var original = entry.Property(nameof(ITenantEntity.TenantId)).OriginalValue;
                var current = entry.Property(nameof(ITenantEntity.TenantId)).CurrentValue;
                if (!Equals(original, current))
                {
                    throw new InvalidOperationException("TenantId cannot be changed after insert.");
                }
            }
        }

        foreach (var entry in context.ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State is EntityState.Added)
            {
                entry.Property(nameof(IAuditable.CreatedAt)).CurrentValue = _clock.UtcNow;
                if (_currentUser.UserId.HasValue)
                {
                    entry.Property(nameof(IAuditable.CreatedBy)).CurrentValue = _currentUser.UserId;
                }
            }

            if (entry.State is EntityState.Modified)
            {
                entry.Property(nameof(IAuditable.UpdatedAt)).CurrentValue = _clock.UtcNow;
                if (_currentUser.UserId.HasValue)
                {
                    entry.Property(nameof(IAuditable.UpdatedBy)).CurrentValue = _currentUser.UserId;
                }

                TryBumpVersion(entry);
            }
        }
    }

    private static void TryBumpVersion(EntityEntry entry)
    {
        var versionProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name == nameof(AuditableEntity.Version));
        if (versionProperty is null || versionProperty.Metadata.ClrType != typeof(long))
        {
            return;
        }

        var current = versionProperty.CurrentValue is long value ? value : 0L;
        versionProperty.CurrentValue = current + 1;
    }
}
