namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class FeeScheduleRepository(AppDbContext context) : IFeeScheduleRepository
{
    public void Add(DefaultMonthlyFeeChange change) => context.DefaultMonthlyFeeChanges.Add(change);

    public void Add(ClientBillingPlanChange change) => context.ClientBillingPlanChanges.Add(change);

    public async Task<IReadOnlyList<DefaultMonthlyFeeChange>> ListDefaultFeeChangesAsync(CancellationToken cancellationToken) =>
        await context.DefaultMonthlyFeeChanges.AsNoTracking()
            .OrderBy(change => change.EffectiveFrom)
            .ToListAsync(cancellationToken);

    public Task<DefaultMonthlyFeeChange?> FindDefaultFeeChangeForUpdateAsync(DateOnly effectiveFrom, CancellationToken cancellationToken) =>
        context.DefaultMonthlyFeeChanges.FirstOrDefaultAsync(change => change.EffectiveFrom == effectiveFrom, cancellationToken);

    public async Task<IReadOnlyList<ClientBillingPlanChange>> ListClientPlanChangesAsync(
        IReadOnlyCollection<Guid> clientIds,
        CancellationToken cancellationToken)
    {
        if (clientIds.Count == 0)
        {
            return [];
        }

        return await context.ClientBillingPlanChanges.AsNoTracking()
            .Where(change => clientIds.Contains(change.ClientId))
            .OrderBy(change => change.EffectiveFrom)
            .ToListAsync(cancellationToken);
    }

    public Task<ClientBillingPlanChange?> FindClientPlanChangeForUpdateAsync(Guid clientId, DateOnly effectiveFrom, CancellationToken cancellationToken) =>
        context.ClientBillingPlanChanges.FirstOrDefaultAsync(
            change => change.ClientId == clientId && change.EffectiveFrom == effectiveFrom,
            cancellationToken);
}
