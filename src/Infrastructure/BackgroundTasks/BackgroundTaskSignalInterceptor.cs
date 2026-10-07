using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ClassManager.Infrastructure.BackgroundTasks;

internal sealed class BackgroundTaskSignalInterceptor(BackgroundTaskSignal signal) : ISaveChangesInterceptor, IDbTransactionInterceptor
{
    private bool _hasUncommittedTasks;

    public ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        NoteAddedTasks(eventData.Context);
        return ValueTask.FromResult(result);
    }

    public InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        NoteAddedTasks(eventData.Context);
        return result;
    }

    public ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        RingUnlessInTransaction(eventData.Context);
        return ValueTask.FromResult(result);
    }

    public int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        RingUnlessInTransaction(eventData.Context);
        return result;
    }

    public Task TransactionCommittedAsync(DbTransaction transaction, TransactionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        RingIfTasksWereAdded();
        return Task.CompletedTask;
    }

    public void TransactionCommitted(DbTransaction transaction, TransactionEndEventData eventData) => RingIfTasksWereAdded();

    public Task TransactionRolledBackAsync(DbTransaction transaction, TransactionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        _hasUncommittedTasks = false;
        return Task.CompletedTask;
    }

    public void TransactionRolledBack(DbTransaction transaction, TransactionEndEventData eventData) => _hasUncommittedTasks = false;

    private void NoteAddedTasks(DbContext? context)
    {
        if (context is not null && context.ChangeTracker.Entries<BackgroundTask>().Any(entry => entry.State == EntityState.Added))
        {
            _hasUncommittedTasks = true;
        }
    }

    private void RingUnlessInTransaction(DbContext? context)
    {
        if (context?.Database.CurrentTransaction is null)
        {
            RingIfTasksWereAdded();
        }
    }

    private void RingIfTasksWereAdded()
    {
        if (_hasUncommittedTasks)
        {
            _hasUncommittedTasks = false;
            signal.Ring();
        }
    }
}
