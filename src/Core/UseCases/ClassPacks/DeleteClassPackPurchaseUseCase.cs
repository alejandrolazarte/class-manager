using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.UseCases.ClassPacks;

public sealed record DeleteClassPackPurchaseCommand(Guid PurchaseId);

public sealed class DeleteClassPackPurchaseUseCase(
    IClassPackPurchaseRepository purchaseRepository,
    IClientRepository clientRepository,
    IUnitOfWork unitOfWork,
    IAccessScopes accessScopes,
    ICurrentMember currentMember)
    : IUseCase<DeleteClassPackPurchaseCommand, ClassPackPurchaseResponse>
{
    public async Task<Result<ClassPackPurchaseResponse>> ExecuteAsync(DeleteClassPackPurchaseCommand command, CancellationToken cancellationToken)
    {
        var purchase = await purchaseRepository.GetForUpdateAsync(command.PurchaseId, cancellationToken);
        if (purchase is null)
        {
            return ClassPackFailures.PurchaseNotFound();
        }

        if (!await MoneyRules.CanUndoAsync(
            accessScopes, clientRepository, currentMember, purchase.ClientId, purchase.RecordedByUserId, cancellationToken))
        {
            return AccessRules.NotYours();
        }

        purchaseRepository.Remove(purchase);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ClassPackPurchaseResponse.From(purchase);
    }
}
