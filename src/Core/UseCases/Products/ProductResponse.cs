using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.UseCases.Products;

public sealed record ProductVariantResponse(Guid Id, string Name, int? Stock);

public sealed record ProductResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    StockMode StockMode,
    bool IsVisibleInApp,
    bool IsActive,
    IReadOnlyList<ProductVariantResponse> Variants)
{
    public static ProductResponse From(Product product, IReadOnlyDictionary<Guid, int> stockByVariant) =>
        new(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.StockMode,
            product.IsVisibleInApp,
            product.IsActive,
            [
                .. product.Variants
                    .Where(variant => variant.IsActive)
                    .OrderBy(variant => variant.Position)
                    .Select(variant => new ProductVariantResponse(
                        variant.Id,
                        variant.Name,
                        product.TracksStock ? stockByVariant.GetValueOrDefault(variant.Id) : null)),
            ]);
}
