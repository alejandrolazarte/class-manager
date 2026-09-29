using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Orders;
using ClassManager.Core.U.Tests.Domain.ClassPacks;
using ClassManager.Core.U.Tests.Domain.Products;

namespace ClassManager.Core.U.Tests.Domain.Orders;

internal static class OrderTestData
{
    public static OrderLine ProductLine(int quantity = 2)
    {
        var product = ProductTestData.Product();
        return OrderLine.ForProduct(product, product.Variants[0], quantity, null).Value!;
    }

    public static OrderLine PackLine() => OrderLine.ForClassPack(ClassPackTestData.Pack(), null).Value!;

    public static Order Requested(DateTimeOffset createdAt, params OrderLine[] lines) =>
        Order.Request(ClassPackTestData.ClientId, lines, null, createdAt).Value!;

    public static Order CounterSale(bool isDelivered, params OrderLine[] lines) =>
        Order.CounterSale(ClassPackTestData.ClientId, lines, PaymentMethod.Cash, TestData.Today, null, isDelivered, TestData.Today, null, TestData.Now).Value!;
}
