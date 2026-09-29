namespace ClassManager.Core.Domain.Products;

public static class ProductErrorCodes
{
    public const string NotFound = "product.not_found";
    public const string NameTaken = "product.name_taken";
    public const string VariantNotFound = "product.variant_not_found";
    public const string StockNotTracked = "product.stock_not_tracked";
    public const string OutOfStock = "product.out_of_stock";
    public const string NotSold = "product.not_sold";
}
