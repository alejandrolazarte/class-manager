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
        orders.MapPut(ApiRoutes.OrderById + ApiRoutes.Payment, ConfirmOrderPaymentAsync).RequirePermission(Permissions.Orders.Manage);
        orders.MapPut(ApiRoutes.OrderById + ApiRoutes.Ready, MarkOrderReadyAsync).RequirePermission(Permissions.Orders.Manage);
        orders.MapPut(ApiRoutes.OrderById + ApiRoutes.Delivery, ChooseOrderDeliveryAsync).RequirePermission(Permissions.Orders.Manage);
        orders.MapGet(ApiRoutes.DeliveryClasses, ListDeliveryClassesAsync).RequirePermission(Permissions.Orders.Manage);
        orders.MapPut(ApiRoutes.OrderById + ApiRoutes.Cancellation, CancelOrderAsync).RequirePermission(Permissions.Orders.Manage);
        orders.MapPut(ApiRoutes.OrderById + ApiRoutes.Delivered, MarkOrderDeliveredAsync).RequirePermission(Permissions.Orders.Manage);
        orders.MapPost(ApiRoutes.OrderById + ApiRoutes.Refunds, RefundOrderAsync).RequirePermission(Permissions.Orders.Manage);

        var classGroups = endpoints.MapGroup(ApiRoutes.ClassGroups);
        classGroups.MapGet(ApiRoutes.ClassGroupById + ApiRoutes.Deliveries, ListClassDeliveriesAsync)
            .RequirePermission(Permissions.Attendance.RecordAll, Permissions.Attendance.RecordOwn);
        classGroups.MapPut(ApiRoutes.ClassGroupById + ApiRoutes.Deliveries + ApiRoutes.OrderById, DeliverInClassAsync)
            .RequirePermission(Permissions.Attendance.RecordAll, Permissions.Attendance.RecordOwn);

        return endpoints;
    }

    private static async Task<IResult> MarkOrderReadyAsync(
        Guid orderId,
        IUseCase<MarkOrderReadyCommand, OrderResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new MarkOrderReadyCommand(orderId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> ChooseOrderDeliveryAsync(
        Guid orderId,
        ChooseOrderDeliveryRequest request,
        IUseCase<ChooseOrderDeliveryCommand, OrderResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request.ToCommand(orderId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> ListDeliveryClassesAsync(
        Guid clientId,
        IUseCase<ListDeliveryClassesQuery, IReadOnlyList<DeliveryClassResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ListDeliveryClassesQuery(clientId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> ListClassDeliveriesAsync(
        Guid classGroupId,
        IUseCase<ListClassDeliveriesQuery, IReadOnlyList<ClassDeliveryResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ListClassDeliveriesQuery(classGroupId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> DeliverInClassAsync(
        Guid classGroupId,
        Guid orderId,
        IUseCase<DeliverInClassCommand, ClassDeliveryResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new DeliverInClassCommand(classGroupId, orderId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> ListOrdersAsync(
        Guid? clientId,
        bool? awaitingPickup,
        bool? requested,
        IUseCase<ListOrdersQuery, IReadOnlyList<OrderResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(
            new ListOrdersQuery(clientId, awaitingPickup ?? false, requested ?? false), cancellationToken);

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

    private static async Task<IResult> ConfirmOrderPaymentAsync(
        Guid orderId,
        ConfirmOrderPaymentRequest request,
        IUseCase<ConfirmOrderPaymentCommand, OrderResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(request.ToCommand(orderId), cancellationToken);

        return result.ToOkResult();
    }

    private static async Task<IResult> CancelOrderAsync(
        Guid orderId,
        IUseCase<CancelOrderCommand, OrderResponse> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new CancelOrderCommand(orderId), cancellationToken);

        return result.ToOkResult();
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
