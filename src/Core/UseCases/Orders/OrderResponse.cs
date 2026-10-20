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

public sealed record OrderPaymentResponse(Guid Id, decimal Amount, PaymentMethod Method, DateOnly PaidOn);

public sealed record OrderResponse(
    Guid Id,
    int Number,
    Guid? ClientId,
    string? ClientFullName,
    OrderChannel Channel,
    OrderStatus Status,
    OrderPaymentStatus PaymentStatus,
    bool AwaitsPickup,
    DateOnly? LastPaidOn,
    string CurrencyCode,
    decimal Total,
    decimal RefundedAmount,
    decimal PaidAmount,
    decimal Balance,
    IReadOnlyList<OrderPaymentResponse> Payments,
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
            order.Number,
            order.ClientId,
            client?.User.FullName,
            order.Channel,
            order.Status,
            order.PaymentStatus,
            order.AwaitsPickup,
            order.LastPaidOn,
            order.CurrencyCode,
            order.Total,
            order.RefundedAmount,
            order.PaidAmount,
            order.Balance,
            [
                .. order.Payments
                    .OrderBy(payment => payment.PaidOn)
                    .ThenBy(payment => payment.CreatedAt)
                    .Select(payment => new OrderPaymentResponse(payment.Id, payment.Amount, payment.Method, payment.PaidOn)),
            ],
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
