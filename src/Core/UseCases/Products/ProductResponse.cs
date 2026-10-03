using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Domain.Products;
using ClassManager.Core.UseCases.Images;

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
    IReadOnlyList<ProductVariantResponse> Variants,
    IReadOnlyList<CatalogImageResponse> Images)
{
    public static ProductResponse From(
        Product product,
        IReadOnlyDictionary<Guid, int> stockByVariant,
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
                        variant.Name,
                        product.TracksStock ? stockByVariant.GetValueOrDefault(variant.Id) : null)),
            ],
            CatalogImageResponse.ListFrom(product.ImagesInOrder, documentStorage));
}
