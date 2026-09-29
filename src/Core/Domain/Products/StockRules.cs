namespace ClassManager.Core.Domain.Products;

public static class StockRules
{
    public static bool CanSell(Product product, int currentStock, int quantity) =>
        product.StockMode switch
        {
            StockMode.Tracked => currentStock >= quantity,
            _ => true,
        };
}
