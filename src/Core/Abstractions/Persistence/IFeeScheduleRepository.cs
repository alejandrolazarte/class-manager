using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IFeeScheduleRepository
{
    void Add(DefaultMonthlyFeeChange change);

    void Add(ClientBillingPlanChange change);

    Task<IReadOnlyList<DefaultMonthlyFeeChange>> ListDefaultFeeChangesAsync(CancellationToken cancellationToken);

    Task<DefaultMonthlyFeeChange?> FindDefaultFeeChangeForUpdateAsync(DateOnly effectiveFrom, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClientBillingPlanChange>> ListClientPlanChangesAsync(IReadOnlyCollection<long> clientIds, CancellationToken cancellationToken);

    Task<IReadOnlyList<long>> ListClientIdsWithoutPlanAsync(CancellationToken cancellationToken);

    Task<ClientBillingPlanChange?> FindClientPlanChangeForUpdateAsync(long clientId, DateOnly effectiveFrom, CancellationToken cancellationToken);
}
