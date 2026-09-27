using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IClassPackPurchaseRepository
{
    void Add(ClassPackPurchase purchase);

    void Remove(ClassPackPurchase purchase);

    Task<ClassPackPurchase?> GetForUpdateAsync(Guid purchaseId, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClassPackPurchase>> ListByClientsAsync(IReadOnlyCollection<Guid> clientIds, CancellationToken cancellationToken);

    Task<bool> IsTrialDeductedAsync(Guid trialLessonId, CancellationToken cancellationToken);

    Task<decimal> SumPriceBetweenAsync(DateOnly firstDay, DateOnly lastDay, CancellationToken cancellationToken);
}
