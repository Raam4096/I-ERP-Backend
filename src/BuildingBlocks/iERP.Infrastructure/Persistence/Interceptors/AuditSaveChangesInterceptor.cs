using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace iERP.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Skeleton interceptor hook for future ActivityLog generation on writes.
/// </summary>
public sealed class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        // Intentionally empty: modules will later project ChangedEntries into ActivityLog.
        _ = eventData.Context?.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
