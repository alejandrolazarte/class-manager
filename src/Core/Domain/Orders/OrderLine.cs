using ClassManager.Core.Common;
using ClassManager.Core.Domain.ClassPacks;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Products;
using ClassManager.Tenancy;

namespace ClassManager.Core.Domain.Orders;

public sealed class OrderLine : ITenantOwned
{
    public const int NameMaxLength = Product.NameMaxLength + ProductVariant.NameMaxLength + 3;
    public const int MaximumQuantity = 99;
    public const string LinesFieldName = "Lines";

    private const string QuantityRangeMessage = "The quantity must be between 1 and 99.";
    private const string RefundQuantityMessage = "You can't return more units than were sold.";

    private OrderLine()
    {
    }

    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid OrderId { get; private set; }
    public OrderLineKind Kind { get; private set; }
    public Guid? ClassPackId { get; private set; }
    public Guid? ClassPackPurchaseId { get; private set; }
    public Guid? ProductId { get; private set; }
    public Guid? ProductVariantId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int RefundedQuantity { get; private set; }
    public decimal RefundedAmount { get; private set; }

    public decimal Total => UnitPrice * Quantity;

    public bool IsFullyRefunded => RefundedQuantity >= Quantity;

    public static Result<OrderLine> ForClassPack(ClassPack pack, decimal? unitPrice)
    {
        var chargedPrice = unitPrice ?? pack.Price;
        var priceValidation = MonthlyFee.Validate(chargedPrice, LinesFieldName);
        if (priceValidation.IsFailure)
        {
            return priceValidation.Error!;
        }

        return new OrderLine
        {
            Id = Guid.CreateVersion7(),
            Kind = OrderLineKind.ClassPack,
            ClassPackId = pack.Id,
            Name = pack.Name,
            Quantity = 1,
            UnitPrice = chargedPrice,
        };
    }

    public static Result<OrderLine> ForProduct(Product product, ProductVariant variant, int? quantity, decimal? unitPrice)
    {
        if (quantity is null or < 1 or > MaximumQuantity)
        {
            return Result.Validation<OrderLine>(QuantityRangeMessage, fieldName: LinesFieldName);
        }

        var chargedPrice = unitPrice ?? product.Price;
        var priceValidation = MonthlyFee.Validate(chargedPrice, LinesFieldName);
        if (priceValidation.IsFailure)
        {
            return priceValidation.Error!;
        }

        return new OrderLine
        {
            Id = Guid.CreateVersion7(),
            Kind = OrderLineKind.Product,
            ProductId = product.Id,
            ProductVariantId = variant.Id,
            Name = product.SaleNameOf(variant),
            Quantity = quantity.Value,
            UnitPrice = chargedPrice,
        };
    }

    public void LinkPurchase(Guid? purchaseId) => ClassPackPurchaseId = purchaseId;

    internal void AttachTo(Guid orderId) => OrderId = orderId;

    internal Result RefundUnits(int? quantity)
    {
        if (quantity is null or < 1 || quantity > Quantity - RefundedQuantity)
        {
            return Result.Validation(RefundQuantityMessage, OrderErrorCodes.NothingToRefund, LinesFieldName);
        }

        RefundedQuantity += quantity.Value;
        RefundedAmount += UnitPrice * quantity.Value;
        return Result.Success();
    }

    internal void RefundClasses(decimal amount)
    {
        RefundedQuantity = Quantity;
        RefundedAmount = amount;
    }
}
