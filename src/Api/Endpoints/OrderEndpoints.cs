using ClassManager.Api.Authentication;
using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.Orders;

namespace ClassManager.Api.Endpoints;

internal static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var orders = endpoints.MapGroup(ApiRoutes.Orders);
        orders.MapGet("/", ListOrdersAsync).RequirePermission(Permissions.Orders.ViewAll, Permissions.Orders.ViewOwn);
        orders.MapPost("/", CreateCounterSaleAsync).RequirePermission(Permissions.Orders.Manage);
        orders.MapPut(ApiRoutes.OrderById + ApiRoutes.Delivered, MarkOrderDeliveredAsync).RequirePermission(Permissions.Orders.Manage);
        orders.MapPost(ApiRoutes.OrderById + ApiRoutes.Refunds, RefundOrderAsync).RequirePermission(Permissions.Orders.Manage);

        return endpoints;
    }

    private static async Task<IResult> ListOrdersAsync(
        Guid? clientId,
        bool? awaitingPickup,
        IUseCase<ListOrdersQuery, IReadOnlyList<OrderResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ListOrdersQuery(clientId, awaitingPickup ?? false), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> CreateCounterSaleAsync(
        CreateCounterSaleCommand command,
        IUseCase<CreateCounterSaleCommand, OrderResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(command, cancellationToken);

        return result.ToHttpResult(order => TypedResults.Created($"{ApiRoutes.Orders}/{order.Id}", order));
    }

    private static async Task<IResult> MarkOrderDeliveredAsync(
        Guid orderId,
        IUseCase<MarkOrderDeliveredCommand, OrderResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new MarkOrderDeliveredCommand(orderId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> RefundOrderAsync(
        Guid orderId,
        RefundOrderRequest request,
        IUseCase<RefundOrderCommand, OrderResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request.ToCommand(orderId), cancellationToken);

        return result.ToOkResult();
    }
}
