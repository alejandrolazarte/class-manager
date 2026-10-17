namespace ClassManager.Core.Abstractions.Persistence;

public interface IPeopleLock
{
    Task LockAsync(CancellationToken cancellationToken);
}
