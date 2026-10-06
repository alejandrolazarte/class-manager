using ClassManager.Records;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ClassManager.Infrastructure.Persistence;

internal sealed class SoftDeleteSaveChangesInterceptor(TimeProvider timeProvider) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        MarkRemovedEntitiesAsDeleted(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        MarkRemovedEntitiesAsDeleted(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void MarkRemovedEntitiesAsDeleted(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var removedEntries = context.ChangeTracker.Entries<ISoftDeletable>()
            .Where(entry => entry.State == EntityState.Deleted)
            .ToList();
        var now = timeProvider.GetUtcNow();
        foreach (var entry in removedEntries)
        {
            entry.State = EntityState.Unchanged;
            entry.Entity.Delete(now);
            entry.DetectChanges();
        }
    }
}
