using ClassManager.Security.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace ClassManager.Infrastructure.Persistence;

internal sealed class UnitOfWork(AppDbContext context, SecurityDbContext securityContext) : IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        context.SaveChangesTranslatingConflictsAsync(cancellationToken);

    public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        await securityContext.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);

        return new SharedTransaction(transaction, securityContext);
    }

    private sealed class SharedTransaction(IDbContextTransaction transaction, SecurityDbContext securityContext) : IUnitOfWorkTransaction
    {
        public Task CommitAsync(CancellationToken cancellationToken) => transaction.CommitAsync(cancellationToken);

        public async ValueTask DisposeAsync()
        {
            await securityContext.Database.UseTransactionAsync(null);
            await transaction.DisposeAsync();
        }
    }
}
