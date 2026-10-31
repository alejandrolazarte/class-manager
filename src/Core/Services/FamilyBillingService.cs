using ClassManager.Core.Abstractions.Fees;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.Services;

public sealed class FamilyBillingService(IMonthlyPlansReader monthlyPlansReader) : IFamilyBillingService
{
    public async Task<IReadOnlyDictionary<long, FamilyMonthBilling>> CalculateAsync(
        IReadOnlyCollection<long> clientIds,
        BillingMonth month,
        CancellationToken cancellationToken)
    {
        var monthlyPlans = await monthlyPlansReader.ReadAsync(clientIds, cancellationToken);
        return clientIds.Distinct().ToDictionary(
            clientId => clientId,
            clientId => FamilyBilling.For(
                clientId,
                monthlyPlans.OfFamily(clientId),
                monthlyPlans.PlansById,
                [.. monthlyPlans.PriceChanges],
                month));
    }
}
