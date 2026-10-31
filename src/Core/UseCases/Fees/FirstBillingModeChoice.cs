using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.UseCases.Fees;

internal static class FirstBillingModeChoice
{
    public static async Task KeepItForRegisteredClientsAsync(
        IFeeScheduleRepository feeScheduleRepository,
        IReadOnlyCollection<DefaultMonthlyFeeChange> defaultFeeChanges,
        DateOnly today,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        foreach (var clientId in await feeScheduleRepository.ListClientIdsWithoutPlanAsync(cancellationToken))
        {
            foreach (var planChange in ClientBillingPlanChange.ForRegisteredClient(clientId, defaultFeeChanges, today, now))
            {
                feeScheduleRepository.Add(planChange);
            }
        }
    }
}
