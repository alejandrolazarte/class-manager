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
        return OrderLine.ForProduct(product, product.Variants[0], quantity, null).Value!.WithId();
    }

    public static OrderLine PackLine() => OrderLine.ForClassPack(ClassPackTestData.Pack(), null).Value!.WithId();

    public static Order Requested(DateTimeOffset createdAt, params OrderLine[] lines) =>
        Order.Request(ClassPackTestData.ClientId, lines, TestData.CurrencyCode, null, createdAt).Value!.WithId();

    public static Order CounterSale(bool isDelivered, params OrderLine[] lines) => PartlyPaidCounterSale(null, isDelivered, lines);

    public static Order PartlyPaidCounterSale(decimal? paidAmount, bool isDelivered, params OrderLine[] lines) =>
        Order.CounterSale(
            ClassPackTestData.ClientId, lines, TestData.CurrencyCode, paidAmount, PaymentMethod.Cash, TestData.Today, null, isDelivered, TestData.Today, null, TestData.Now).Value!.WithId();

    public static Result Pay(Order order, decimal amount)
    {
        var payment = order.RecordPayment(amount, PaymentMethod.Transfer, TestData.Today, TestData.Today, null, TestData.Now);
        order.WithChildIds();
        return payment;
    }
}
