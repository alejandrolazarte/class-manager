namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class ClassPackPurchaseRepository(AppDbContext context) : IClassPackPurchaseRepository
{
    public void Add(ClassPackPurchase purchase) => context.ClassPackPurchases.Add(purchase);

    public void Remove(ClassPackPurchase purchase) => context.ClassPackPurchases.Remove(purchase);

    public Task<ClassPackPurchase?> GetForUpdateAsync(Guid purchaseId, CancellationToken cancellationToken) =>
        context.ClassPackPurchases.FirstOrDefaultAsync(purchase => purchase.Id == purchaseId, cancellationToken);

    public async Task<IReadOnlyList<ClassPackPurchase>> ListByClientsAsync(IReadOnlyCollection<Guid> clientIds, CancellationToken cancellationToken)
    {
        if (clientIds.Count == 0)
        {
            return [];
        }

        return await context.ClassPackPurchases.AsNoTracking()
            .Where(purchase => clientIds.Contains(purchase.ClientId))
            .ToListAsync(cancellationToken);
    }

    public Task<bool> IsTrialDeductedAsync(Guid trialLessonId, CancellationToken cancellationToken) =>
        context.ClassPackPurchases.AsNoTracking().AnyAsync(purchase => purchase.TrialLessonId == trialLessonId, cancellationToken);
}
