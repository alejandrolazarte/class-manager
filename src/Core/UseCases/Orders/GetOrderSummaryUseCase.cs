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
        var orders = await orderRepository.ListPaidBetweenOrRequestedAsync(month.Value!.FirstDay, month.Value.LastDay, scope, cancellationToken);
        var paidLines = orders
            .Where(order => order.Status is OrderStatus.Paid or OrderStatus.Delivered)
            .SelectMany(order => order.Lines)
            .ToList();

        return new OrderSummaryResponse(
            month.Value.ToString(),
            paidLines.Sum(NetAmountOf),
            orders.Where(order => order.Status == OrderStatus.Requested).Sum(order => order.Total),
            paidLines.Where(line => line.Kind == OrderLineKind.ClassPack).Sum(NetAmountOf));
    }

    private static decimal NetAmountOf(OrderLine line) => line.Total - line.RefundedAmount;
}
