using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.Abstractions.Fees;

public interface IFamilyBillingService
{
    Task<IReadOnlyDictionary<long, FamilyMonthBilling>> CalculateAsync(
        IReadOnlyCollection<long> clientIds,
        BillingMonth month,
        CancellationToken cancellationToken);
}
