using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.Abstractions.Fees;

public interface IMonthlyPlansReader
{
    Task<MonthlyPlans> ReadAsync(IReadOnlyCollection<long> clientIds, CancellationToken cancellationToken);
}
