namespace ClassManager.Core.Abstractions.Persistence;

public interface IStockLock
{
    Task LockAsync(IReadOnlyCollection<Guid> variantIds, CancellationToken cancellationToken);
}
