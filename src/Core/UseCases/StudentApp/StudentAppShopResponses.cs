using ClassManager.Core.Domain.Orders;
using ClassManager.Core.Domain.Products;
using ClassManager.Core.UseCases.Images;
using ClassManager.Core.UseCases.Orders;

namespace ClassManager.Core.UseCases.StudentApp;

public sealed record StudentAppShopPackResponse(Guid Id, string Name, string? Description, int ClassCount, decimal Price, int? ValidityMonths, IReadOnlyList<CatalogImageResponse> Images);

public sealed record StudentAppShopVariantResponse(Guid Id, string Name, StockAvailability Availability);

public sealed record StudentAppShopProductResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    IReadOnlyList<StudentAppShopVariantResponse> Variants,
    IReadOnlyList<CatalogImageResponse> Images);

public sealed record StudentAppShopResponse(
    string CurrencyCode,
    IReadOnlyList<StudentAppShopPackResponse> Packs,
    IReadOnlyList<StudentAppShopProductResponse> Products,
    IReadOnlyList<DeliveryClassResponse> DeliveryClasses);

public sealed record StudentAppOrderLineResponse(Guid Id, OrderLineKind Kind, string Name, int Quantity, decimal Total, int RefundedQuantity);

public sealed record StudentAppOrderResponse(
    Guid Id,
    int Number,
    OrderStatus Status,
    bool AwaitsPickup,
    decimal Total,
    decimal RefundedAmount,
    DateTimeOffset CreatedAt,
    DateOnly? PaidOn,
    DateTimeOffset ExpiresAt,
    IReadOnlyList<StudentAppOrderLineResponse> Lines,
    DeliveryMethod Delivery,
    string? DeliveryClassGroupName,
    bool IsReady)
{
    public static StudentAppOrderResponse From(Order order, string? deliveryClassGroupName) =>
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
                .. order.Lines.Select(line => new StudentAppOrderLineResponse(
                    line.Id, line.Kind, line.Name, line.Quantity, line.Total, line.RefundedQuantity)),
            ],
            order.Delivery,
            deliveryClassGroupName,
            order.IsReady);
}
