using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Orders;
using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.UseCases.Orders;

internal static class OrderPayments
{
    public static Result CreditClassPacks(
        Order order,
        IReadOnlyDictionary<Guid, ClassPack> packs,
        IClassPackPurchaseRepository purchaseRepository,
        DateOnly today,
        DateTimeOffset now,
        Guid? recordedByUserId)
    {
        foreach (var line in order.Lines.Where(line => line.Kind == OrderLineKind.ClassPack))
        {
            if (!packs.TryGetValue(line.ClassPackId!.Value, out var pack))
            {
                return Result.Failure(OrderFailures.PackGone());
            }

            var purchase = ClassPackPurchase.Sell(
                order.ClientId!.Value, pack, line.UnitPrice, order.PaidOn!.Value, order.Method, order.Notes, today, now, recordedByUserId);
            if (purchase.IsFailure)
            {
                return Result.Failure(purchase.Error!);
            }

            purchaseRepository.Add(purchase.Value!);
            line.LinkPurchase(purchase.Value!.Id);
        }

        return Result.Success();
    }

    public static async Task ReleaseReservationsAsync(
        Order order,
        IStockMovementRepository stockMovementRepository,
        Guid? recordedByUserId,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        foreach (var reservation in await stockMovementRepository.ListReservationsByOrderAsync(order.Id, cancellationToken))
        {
            stockMovementRepository.Add(StockMovement.Release(reservation, recordedByUserId, now));
        }
    }
}
