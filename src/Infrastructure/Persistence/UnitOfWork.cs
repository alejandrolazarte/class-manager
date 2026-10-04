using ClassManager.Security.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace ClassManager.Infrastructure.Persistence;

internal sealed class UnitOfWork(AppDbContext context, SecurityDbContext securityContext) : IUnitOfWork
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
        await securityContext.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);

        return new SharedTransaction(this, transaction, securityContext);
    }

    private sealed class SharedTransaction(UnitOfWork unitOfWork, IDbContextTransaction transaction, SecurityDbContext securityContext)
        : IUnitOfWorkTransaction
    {
        public Task CommitAsync(CancellationToken cancellationToken) =>
            unitOfWork._rollbackRequested ? transaction.RollbackAsync(cancellationToken) : transaction.CommitAsync(cancellationToken);

        public async ValueTask DisposeAsync()
        {
            await securityContext.Database.UseTransactionAsync(null);
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
