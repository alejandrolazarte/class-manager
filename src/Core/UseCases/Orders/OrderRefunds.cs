using ClassManager.Core.Abstractions.Fees;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.UseCases.Orders;

internal static class OrderRefunds
{
    public static async Task<Result> RefundClassesAsync(
        Order order,
        OrderLine line,
        IClassPackPurchaseRepository purchaseRepository,
        IClassBalanceService classBalanceService,
        CancellationToken cancellationToken)
    {
        var purchase = line.ClassPackPurchaseId is { } purchaseId ? await purchaseRepository.GetForUpdateAsync(purchaseId, cancellationToken) : null;
        if (purchase is null)
        {
            return order.RefundUnusedClasses(line, 0, 0);
        }

        var balances = await classBalanceService.CalculateAsync([purchase.ClientId], cancellationToken);
        var usedClasses = balances[purchase.ClientId].Purchases
            .First(usage => usage.Purchase.Id == purchase.Id)
            .UsedClasses;
        var refund = order.RefundUnusedClasses(line, purchase.ClassCount, purchase.ClassCount - usedClasses);
        if (refund.IsFailure)
        {
            return refund;
        }

        if (usedClasses == 0)
        {
            line.LinkPurchase(null);
            purchaseRepository.Remove(purchase);
        }
        else
        {
            purchase.KeepUsedClasses(usedClasses, refund.Value);
        }

        return Result.Success();
    }
}
