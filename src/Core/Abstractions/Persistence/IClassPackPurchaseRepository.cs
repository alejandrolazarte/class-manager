using ClassManager.Core.Domain.ClassPacks;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IClassPackPurchaseRepository
{
    void Add(ClassPackPurchase purchase);

    void Remove(ClassPackPurchase purchase);

    Task<ClassPackPurchase?> GetForUpdateAsync(long purchaseId, CancellationToken cancellationToken);

    Task<IReadOnlyList<ClassPackPurchase>> ListByClientsAsync(IReadOnlyCollection<long> clientIds, CancellationToken cancellationToken);

    Task<bool> IsTrialDeductedAsync(long trialLessonId, CancellationToken cancellationToken);
}
