using ClassManager.Core.Domain.Clients;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Orders;

namespace ClassManager.Core.UseCases.Orders;

public sealed record OrderLineResponse(
    Guid Id,
    OrderLineKind Kind,
    string Name,
    int Quantity,
    decimal UnitPrice,
    decimal Total,
    int RefundedQuantity,
    decimal RefundedAmount,
    Guid? ClassPackPurchaseId);

public sealed record OrderResponse(
    Guid Id,
    Guid? ClientId,
    string? ClientFullName,
    OrderChannel Channel,
    OrderStatus Status,
    bool AwaitsPickup,
    PaymentMethod? Method,
    DateOnly? PaidOn,
    decimal Total,
    decimal RefundedAmount,
    string? Notes,
    DateTimeOffset CreatedAt,
    DateTimeOffset? DeliveredAt,
    IReadOnlyList<OrderLineResponse> Lines,
    DateTimeOffset? CancelledAt,
    DeliveryMethod Delivery,
    Guid? DeliveryClassGroupId,
    string? DeliveryClassGroupName,
    DateTimeOffset? ReadyAt)
{
    public static OrderResponse From(Order order, Client? client, string? deliveryClassGroupName) =>
        new(
            order.Id,
            order.ClientId,
            client?.FullName,
            order.Channel,
            order.Status,
            order.AwaitsPickup,
            order.Method,
            order.PaidOn,
            order.Total,
            order.RefundedAmount,
            order.Notes,
            order.CreatedAt,
            order.DeliveredAt,
            [
                .. order.Lines.Select(line => new OrderLineResponse(
                    line.Id,
                    line.Kind,
                    line.Name,
                    line.Quantity,
                    line.UnitPrice,
                    line.Total,
                    line.RefundedQuantity,
                    line.RefundedAmount,
                    line.ClassPackPurchaseId)),
            ],
            order.CancelledAt,
            order.Delivery,
            order.DeliveryClassGroupId,
            deliveryClassGroupName,
            order.ReadyAt);
}
