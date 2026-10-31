using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.Abstractions.Fees;

public interface IClassBalanceService
{
    Task<IReadOnlyDictionary<long, ClassBalance>> CalculateAsync(IReadOnlyCollection<long> clientIds, CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<long, ClassBalance>> CalculateAsync(IReadOnlyCollection<long> clientIds, DateOnly today, CancellationToken cancellationToken);
}
