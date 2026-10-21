using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.UseCases.Orders;

internal static class OrderDeliveries
{
    public static async Task<Result> DeliverAsync(
        Order order,
        IClassPackRepository classPackRepository,
        IClassPackPurchaseRepository purchaseRepository,
        DateOnly creditedOn,
        DateTimeOffset now,
        Guid? deliveredByUserId,
        CancellationToken cancellationToken)
    {
        var delivery = order.MarkDelivered(deliveredByUserId, now);
        return delivery.IsFailure
            ? delivery
            : await CreditClassPacksAsync(order, classPackRepository, purchaseRepository, creditedOn, now, deliveredByUserId, cancellationToken);
    }

    public static async Task<Result> CreditClassPacksAsync(
        Order order,
        IClassPackRepository classPackRepository,
        IClassPackPurchaseRepository purchaseRepository,
        DateOnly creditedOn,
        DateTimeOffset now,
        Guid? recordedByUserId,
        CancellationToken cancellationToken)
    {
        var packs = await OrderDrafts.LoadPacksAsync(order.Lines.Select(line => line.ClassPackId), classPackRepository, cancellationToken);
        var method = order.Payments.OrderBy(payment => payment.PaidOn).LastOrDefault()?.Method;
        foreach (var line in order.Lines.Where(line => line.Kind == OrderLineKind.ClassPack && line.ClassPackPurchaseId is null && !line.IsFullyRefunded))
        {
            if (!packs.TryGetValue(line.ClassPackId!.Value, out var pack))
            {
                return Result.Failure(OrderFailures.PackGone());
            }

            var purchase = ClassPackPurchase.ForOrder(
                order.ClientId!.Value, pack, line.UnitPrice, order.CurrencyCode, creditedOn, method, order.Notes, now, recordedByUserId);
            if (purchase.IsFailure)
            {
                return Result.Failure(purchase.Error!);
            }

            purchaseRepository.Add(purchase.Value!);
            line.LinkPurchase(purchase.Value!.Id);
        }

        return Result.Success();
    }
}
