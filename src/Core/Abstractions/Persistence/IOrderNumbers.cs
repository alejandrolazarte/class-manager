namespace ClassManager.Core.Abstractions.Persistence;

public interface IOrderNumbers
{
    Task<int> TakeNextAsync(CancellationToken cancellationToken);
}
