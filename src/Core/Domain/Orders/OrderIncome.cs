using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.Domain.Orders;

public sealed record OrderIncome(decimal Collected, decimal Unpaid, decimal ClassPacksCollected, decimal ProductsCollected)
{
    public static OrderIncome In(IEnumerable<Order> orders, BillingMonth month, string currencyCode)
    {
        var countedOrders = orders
            .Where(order => order.CurrencyCode == currencyCode && order.Status != OrderStatus.Cancelled)
            .Select(order => (Collected: CollectedIn(order, month), order.Balance, ClassPackShare: ClassPackShareOf(order)))
            .ToList();
        var collected = Rounded(countedOrders.Sum(order => order.Collected));
        var classPacksCollected = Rounded(countedOrders.Sum(order => order.Collected * order.ClassPackShare));

        return new OrderIncome(
            collected,
            Rounded(countedOrders.Sum(order => order.Balance)),
            classPacksCollected,
            collected - classPacksCollected);
    }

    private static decimal CollectedIn(Order order, BillingMonth month)
    {
        var uncollectedRemainder = order.CollectedAmount;
        var collectedInMonth = 0m;
        foreach (var payment in order.Payments.OrderBy(payment => payment.PaidOn).ThenBy(payment => payment.CreatedAt))
        {
            var collected = Math.Min(payment.Amount, uncollectedRemainder);
            uncollectedRemainder -= collected;
            if (payment.PaidOn >= month.FirstDay && payment.PaidOn <= month.LastDay)
            {
                collectedInMonth += collected;
            }
        }

        return collectedInMonth;
    }

    private static decimal ClassPackShareOf(Order order) =>
        order.AmountDue == 0
            ? 0
            : order.Lines.Where(line => line.Kind == OrderLineKind.ClassPack).Sum(NetAmountOf) / order.AmountDue;

    private static decimal NetAmountOf(OrderLine line) => line.Total - line.RefundedAmount;

    private static decimal Rounded(decimal amount) => decimal.Round(amount, MonthlyFee.AmountDecimals);
}
