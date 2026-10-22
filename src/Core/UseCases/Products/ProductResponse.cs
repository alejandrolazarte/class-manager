using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Domain.Products;
using ClassManager.Core.UseCases.Images;

namespace ClassManager.Core.UseCases.Products;

public sealed record ProductVariantResponse(long Id, string Size, string? Color, int? Stock);

public sealed record ProductResponse(
    long Id,
    string Name,
    string? Description,
    decimal Price,
    StockMode StockMode,
    bool IsVisibleInApp,
    bool IsActive,
    IReadOnlyList<ProductVariantResponse> Variants,
    IReadOnlyList<CatalogImageResponse> Images)
{
    public static ProductResponse From(
        Product product,
        IReadOnlyDictionary<long, int> stockByVariant,
        IDocumentStorageService documentStorage) =>
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
                        variant.Size,
                        variant.Color,
                        product.TracksStock ? stockByVariant.GetValueOrDefault(variant.Id) : null)),
            ],
            CatalogImageResponse.ListFrom(product.ImagesInOrder, documentStorage));
}
