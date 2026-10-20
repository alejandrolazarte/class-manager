using ClassManager.Security.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace ClassManager.Infrastructure.Persistence;

internal sealed class UnitOfWork(
    AppDbContext context,
    SecurityDbContext securityContext,
    IEnumerable<ISharedTransactionContext> sharedContexts)
    : IUnitOfWork
{
    private bool _rollbackRequested;

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        context.SaveChangesTranslatingConflictsAsync(cancellationToken);

    public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        if (context.Database.CurrentTransaction is not null)
        {
            return new JoinedTransaction(this);
        }

        _rollbackRequested = false;
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        var enlistedDatabases = sharedContexts.Select(sharedContext => sharedContext.Database).Prepend(securityContext.Database).ToList();
        foreach (var database in enlistedDatabases)
        {
            await database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);
        }

        return new SharedTransaction(this, transaction, enlistedDatabases);
    }

    private sealed class SharedTransaction(UnitOfWork unitOfWork, IDbContextTransaction transaction, IReadOnlyList<DatabaseFacade> enlistedDatabases)
        : IUnitOfWorkTransaction
    {
        public Task CommitAsync(CancellationToken cancellationToken) =>
            unitOfWork._rollbackRequested ? transaction.RollbackAsync(cancellationToken) : transaction.CommitAsync(cancellationToken);

        public async ValueTask DisposeAsync()
        {
            foreach (var database in enlistedDatabases)
            {
                await database.UseTransactionAsync(null);
            }

            await transaction.DisposeAsync();
        }
    }

    private sealed class JoinedTransaction(UnitOfWork unitOfWork) : IUnitOfWorkTransaction
    {
        private bool _committed;

        public Task CommitAsync(CancellationToken cancellationToken)
        {
            _committed = true;

            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            if (!_committed)
            {
                unitOfWork._rollbackRequested = true;
            }

            return ValueTask.CompletedTask;
        }
    }
}
