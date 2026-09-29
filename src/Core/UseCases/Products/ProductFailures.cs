using ClassManager.Core.Common;
using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.UseCases.Products;

internal static class ProductFailures
{
    private const string NotFoundMessage = "The product does not exist.";
    private const string NameTakenMessage = "Another product already has this name.";
    private const string VariantNotFoundMessage = "The size or variant does not exist.";
    private const string NotSoldMessage = "This product or size is no longer sold.";
    private const string OutOfStockMessage = "There isn't enough stock of this product.";
    private const string ProductNameDetail = "productName";

    public static ResultError NotFound() => new(ProductErrorCodes.NotFound, NotFoundMessage, ErrorKind.NotFound);

    public static ResultError NameTaken() => new(ProductErrorCodes.NameTaken, NameTakenMessage, ErrorKind.Conflict);

    public static ResultError VariantNotFound() => new(ProductErrorCodes.VariantNotFound, VariantNotFoundMessage, ErrorKind.NotFound);

    public static ResultError NotSold() => new(ProductErrorCodes.NotSold, NotSoldMessage, ErrorKind.Validation);

    public static ResultError OutOfStock(string productName) =>
        new(ProductErrorCodes.OutOfStock, OutOfStockMessage, ErrorKind.Conflict)
        {
            Details = new Dictionary<string, object?> { [ProductNameDetail] = productName },
        };
}
