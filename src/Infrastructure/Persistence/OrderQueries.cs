using System.Linq.Expressions;

namespace ClassManager.Infrastructure.Persistence;

internal static class OrderQueries
{
    public static Expression<Func<Order, bool>> IsOwing() =>
        order => order.Status != OrderStatus.Cancelled
            && order.Lines.Sum(line => line.UnitPrice * line.Quantity - line.RefundedAmount) > order.Payments.Sum(payment => payment.Amount);

    public static Expression<Func<Order, bool>> IsUnpaidRequest() =>
        order => order.Channel == OrderChannel.App && order.Status == OrderStatus.Pending && !order.Payments.Any();

    public static Expression<Func<Order, bool>> IsUnpaidRequestCreatedBefore(DateTimeOffset createdBefore) =>
        order => order.Channel == OrderChannel.App && order.Status == OrderStatus.Pending && !order.Payments.Any()
            && order.CreatedAt <= createdBefore;
}
