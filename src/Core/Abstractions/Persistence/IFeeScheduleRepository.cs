using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IFeeScheduleRepository
{
    void Add(DefaultMonthlyFeeChange change);

    void Add(ClientBillingPlanChange change);

    Task<IReadOnlyList<DefaultMonthlyFeeChange>> ListDefaultFeeChangesAsync(CancellationToken cancellationToken);

    Task<DefaultMonthlyFeeChange?> FindDefaultFeeChangeForUpdateAsync(DateOnly effectiveFrom, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClientBillingPlanChange>> ListClientPlanChangesAsync(IReadOnlyCollection<Guid> clientIds, CancellationToken cancellationToken);

    Task<ClientBillingPlanChange?> FindClientPlanChangeForUpdateAsync(Guid clientId, DateOnly effectiveFrom, CancellationToken cancellationToken);
}
