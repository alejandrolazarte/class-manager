using ClassManager.Core.Domain.Orders;
using ClassManager.Core.Domain.Products;
using ClassManager.Core.UseCases.Orders;

namespace ClassManager.Core.UseCases.Families;

public sealed record FamilyShopPackResponse(Guid Id, string Name, int ClassCount, decimal Price, int? ValidityMonths);

public sealed record FamilyShopVariantResponse(Guid Id, string Name, StockAvailability Availability);

public sealed record FamilyShopProductResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    IReadOnlyList<FamilyShopVariantResponse> Variants);

public sealed record FamilyShopResponse(
    string CurrencyCode,
    IReadOnlyList<FamilyShopPackResponse> Packs,
    IReadOnlyList<FamilyShopProductResponse> Products,
    IReadOnlyList<DeliveryClassResponse> DeliveryClasses);

public sealed record FamilyOrderLineResponse(Guid Id, OrderLineKind Kind, string Name, int Quantity, decimal Total, int RefundedQuantity);

public sealed record FamilyOrderResponse(
    Guid Id,
    int Number,
    OrderStatus Status,
    bool AwaitsPickup,
    decimal Total,
    decimal RefundedAmount,
    DateTimeOffset CreatedAt,
    DateOnly? PaidOn,
    DateTimeOffset ExpiresAt,
    IReadOnlyList<FamilyOrderLineResponse> Lines,
    DeliveryMethod Delivery,
    string? DeliveryClassGroupName,
    bool IsReady)
{
    public static FamilyOrderResponse From(Order order, string? deliveryClassGroupName) =>
        new(
            order.Id,
            order.Number,
            order.Status,
            order.AwaitsPickup,
            order.Total,
            order.RefundedAmount,
            order.CreatedAt,
            order.PaidOn,
            order.CreatedAt + Order.RequestLifetime,
            [
                .. order.Lines.Select(line => new FamilyOrderLineResponse(
                    line.Id, line.Kind, line.Name, line.Quantity, line.Total, line.RefundedQuantity)),
            ],
            order.Delivery,
            deliveryClassGroupName,
            order.IsReady);
}
