using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.ClassPacks;

public sealed record DeleteClassPackPurchaseCommand(Guid PurchaseId);

public sealed class DeleteClassPackPurchaseUseCase(IClassPackPurchaseRepository purchaseRepository, IUnitOfWork unitOfWork)
    : IUseCase<DeleteClassPackPurchaseCommand, ClassPackPurchaseResponse>
{
    public async Task<Result<ClassPackPurchaseResponse>> ExecuteAsync(DeleteClassPackPurchaseCommand command, CancellationToken cancellationToken)
    {
        var purchase = await purchaseRepository.GetForUpdateAsync(command.PurchaseId, cancellationToken);
        if (purchase is null)
        {
            return ClassPackFailures.PurchaseNotFound();
        }

        purchaseRepository.Remove(purchase);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ClassPackPurchaseResponse.From(purchase);
    }
}
