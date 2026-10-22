using ClassManager.Core.Abstractions.Security;
using ClassManager.Records;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ClassManager.Infrastructure.Persistence;

internal sealed class AuditSaveChangesInterceptor(TimeProvider timeProvider, ICurrentUser currentUser) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        StampWhoDidWhat(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        StampWhoDidWhat(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void StampWhoDidWhat(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var now = timeProvider.GetUtcNow();
        var userId = currentUser.UserId;
        foreach (var entry in context.ChangeTracker.Entries<ISoftDeletable>().Where(entry => IsBeingSoftDeleted(entry)))
        {
            SetWhenEmpty(entry.Property(nameof(ISoftDeletable.DeletedByUserId)), userId);
        }

        foreach (var entry in context.ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Added)
            {
                StampCreated(entry, now, userId);
            }
            else if (entry.State == EntityState.Modified && !IsBeingSoftDeleted(entry))
            {
                StampUpdated(entry, now, userId);
            }
        }
    }

    private static void StampCreated(EntityEntry entry, DateTimeOffset now, long? userId)
    {
        var createdAt = entry.Property(nameof(IAuditable.CreatedAt));
        SetWhenDefault(createdAt, now);
        var createdByUserId = entry.Property(nameof(IAuditable.CreatedByUserId));
        SetWhenEmpty(createdByUserId, userId);
        SetWhenDefault(entry.Property(nameof(IAuditable.UpdatedAt)), (DateTimeOffset)createdAt.CurrentValue!);
        SetWhenEmpty(entry.Property(nameof(IAuditable.UpdatedByUserId)), (long?)createdByUserId.CurrentValue);
    }

    private static void StampUpdated(EntityEntry entry, DateTimeOffset now, long? userId)
    {
        var updatedAt = entry.Property(nameof(IAuditable.UpdatedAt));
        if (!updatedAt.IsModified)
        {
            updatedAt.CurrentValue = now;
        }

        var updatedByUserId = entry.Property(nameof(IAuditable.UpdatedByUserId));
        if (!updatedByUserId.IsModified)
        {
            updatedByUserId.CurrentValue = userId;
        }
    }

    private static void SetWhenDefault(PropertyEntry property, DateTimeOffset value)
    {
        if ((DateTimeOffset)property.CurrentValue! == default)
        {
            property.CurrentValue = value;
        }
    }

    private static void SetWhenEmpty(PropertyEntry property, long? value)
    {
        property.CurrentValue ??= value;
    }

    private static bool IsBeingSoftDeleted(EntityEntry entry) =>
        entry.Entity is ISoftDeletable { IsDeleted: true }
        && entry.State == EntityState.Modified
        && entry.Property(nameof(ISoftDeletable.IsDeleted)).IsModified;
}
