namespace ClassManager.Core.Domain.Products;

public static class StockRules
{
    public static bool CanSell(Product product, int currentStock, int quantity) =>
        product.StockMode switch
        {
            StockMode.Tracked => currentStock >= quantity,
            _ => true,
        };

    public static StockAvailability AvailabilityOf(Product product, int currentStock) =>
        product.StockMode switch
        {
            StockMode.Unlimited => StockAvailability.Available,
            _ when currentStock > 0 => StockAvailability.Available,
            StockMode.TrackedWithBackorder => StockAvailability.OnOrder,
            _ => StockAvailability.SoldOut,
        };
}
