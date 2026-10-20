using ClassManager.Core.Abstractions.Fees;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Time;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.UseCases.Orders;

public sealed record GetOrderSummaryQuery(string? Month) : IQuery;

public sealed record OrderSummaryResponse(string Month, decimal Collected, decimal Unpaid, decimal ClassPackSales);

public sealed class GetOrderSummaryUseCase(
    IOrderRepository orderRepository,
    IBusinessCalendarService businessCalendar,
    IBusinessCurrencyService businessCurrency,
    IAccessScopes accessScopes)
    : IUseCase<GetOrderSummaryQuery, OrderSummaryResponse>
{
    public async Task<Result<OrderSummaryResponse>> ExecuteAsync(GetOrderSummaryQuery command, CancellationToken cancellationToken)
    {
        var month = command.Month is null
            ? BillingMonth.From(await businessCalendar.TodayAsync(cancellationToken))
            : BillingMonth.Parse(command.Month, nameof(GetOrderSummaryQuery.Month));
        if (month.IsFailure)
        {
            return month.Error!;
        }

        var scope = await accessScopes.ForClientsAsync(Permissions.Orders.ViewAll, cancellationToken);
        var currencyCode = await businessCurrency.CurrencyCodeAsync(cancellationToken);
        var orders = (await orderRepository.ListPaidBetweenOrOwingAsync(month.Value!.FirstDay, month.Value.LastDay, scope, cancellationToken))
            .Where(order => order.CurrencyCode == currencyCode && order.Status != OrderStatus.Cancelled)
            .ToList();
        var collectedByOrder = orders
            .Select(order => (Order: order, Collected: CollectedIn(order, month.Value)))
            .ToList();

        return new OrderSummaryResponse(
            month.Value.ToString(),
            collectedByOrder.Sum(collected => collected.Collected),
            orders.Sum(order => order.Balance),
            decimal.Round(collectedByOrder.Sum(collected => collected.Collected * ClassPackShareOf(collected.Order)), MonthlyFee.AmountDecimals));
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
}
