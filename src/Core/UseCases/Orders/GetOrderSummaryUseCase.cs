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

public sealed record OrderSummaryResponse(string Month, decimal Collected, decimal Unpaid);

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
        var orders = await orderRepository.ListPaidBetweenOrOwingAsync(month.Value!.FirstDay, month.Value.LastDay, scope, cancellationToken);
        var income = OrderIncome.In(orders, month.Value, currencyCode);

        return new OrderSummaryResponse(month.Value.ToString(), income.ProductsCollected, income.ProductsUnpaid);
    }
}
