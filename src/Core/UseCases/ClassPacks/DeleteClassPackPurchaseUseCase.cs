using ClassManager.Core.Abstractions.Fees;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Common;
using ClassManager.Core.UseCases.Fees;
using ClassManager.Core.UseCases.Orders;

namespace ClassManager.Core.UseCases.ClassPacks;

public sealed record DeleteClassPackPurchaseCommand(Guid PurchaseId);

public sealed class DeleteClassPackPurchaseUseCase(
    IClassPackPurchaseRepository purchaseRepository,
    IClientRepository clientRepository,
    IOrderRepository orderRepository,
    IClassBalanceService classBalanceService,
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

        var order = await orderRepository.GetForUpdateByPurchaseAsync(purchase.Id, cancellationToken);
        if (order is null)
        {
            purchaseRepository.Remove(purchase);
        }
        else
        {
            var line = order.Lines.First(line => line.ClassPackPurchaseId == purchase.Id);
            var refund = await OrderRefunds.RefundClassesAsync(order, line, purchaseRepository, classBalanceService, cancellationToken);
            if (refund.IsFailure)
            {
                return refund.Error!;
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ClassPackPurchaseResponse.From(purchase);
    }
}
