using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.Abstractions.Fees;

public interface IClassBalanceService
{
    Task<IReadOnlyDictionary<Guid, ClassBalance>> CalculateAsync(IReadOnlyCollection<Guid> clientIds, CancellationToken cancellationToken);
}
