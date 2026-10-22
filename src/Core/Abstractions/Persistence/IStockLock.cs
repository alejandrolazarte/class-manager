namespace ClassManager.Core.Abstractions.Persistence;

public interface IStockLock
{
    Task LockAsync(IReadOnlyCollection<long> variantIds, CancellationToken cancellationToken);
}
