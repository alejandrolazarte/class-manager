using ClassManager.Core.Abstractions.Images;
using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Documents;
using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.UseCases.Products;

public sealed record AddProductImageCommand(Guid ProductId, byte[] Content) : ICommand;

public sealed record RemoveProductImageCommand(Guid ProductId, Guid DocumentId) : ICommand;

public sealed record ReorderProductImagesCommand(Guid ProductId, IReadOnlyList<Guid>? DocumentIds) : ICommand;

public sealed class AddProductImageUseCase(
    IProductRepository productRepository,
    IStockMovementRepository stockMovementRepository,
    IDocumentStorageService documentStorage,
    ICatalogImageService catalogImageService)
    : IUseCase<AddProductImageCommand, ProductResponse>
{
    public Task<Result<ProductResponse>> ExecuteAsync(AddProductImageCommand command, CancellationToken cancellationToken) =>
        ProductImageEditing.ChangeAsync(
            productRepository,
            stockMovementRepository,
            documentStorage,
            command.ProductId,
            product => catalogImageService.AddAsync(product, DocumentOwner.Product, command.Content, cancellationToken),
            cancellationToken);
}

public sealed class RemoveProductImageUseCase(
    IProductRepository productRepository,
    IStockMovementRepository stockMovementRepository,
    IDocumentStorageService documentStorage,
    ICatalogImageService catalogImageService)
    : IUseCase<RemoveProductImageCommand, ProductResponse>
{
    public Task<Result<ProductResponse>> ExecuteAsync(RemoveProductImageCommand command, CancellationToken cancellationToken) =>
        ProductImageEditing.ChangeAsync(
            productRepository,
            stockMovementRepository,
            documentStorage,
            command.ProductId,
            product => catalogImageService.RemoveAsync(product, command.DocumentId, cancellationToken),
            cancellationToken);
}

public sealed class ReorderProductImagesUseCase(
    IProductRepository productRepository,
    IStockMovementRepository stockMovementRepository,
    IDocumentStorageService documentStorage,
    ICatalogImageService catalogImageService)
    : IUseCase<ReorderProductImagesCommand, ProductResponse>
{
    public Task<Result<ProductResponse>> ExecuteAsync(ReorderProductImagesCommand command, CancellationToken cancellationToken) =>
        ProductImageEditing.ChangeAsync(
            productRepository,
            stockMovementRepository,
            documentStorage,
            command.ProductId,
            product => catalogImageService.ReorderAsync(product, command.DocumentIds, cancellationToken),
            cancellationToken);
}

internal static class ProductImageEditing
{
    public static async Task<Result<ProductResponse>> ChangeAsync(
        IProductRepository productRepository,
        IStockMovementRepository stockMovementRepository,
        IDocumentStorageService documentStorage,
        Guid productId,
        Func<Product, Task<Result>> change,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.GetForUpdateAsync(productId, cancellationToken);
        if (product is null)
        {
            return ProductFailures.NotFound();
        }

        var result = await change(product);
        if (result.IsFailure)
        {
            return result.Error!;
        }

        var stockByVariant = await stockMovementRepository.StockByVariantAsync([.. product.Variants.Select(variant => variant.Id)], cancellationToken);
        return ProductResponse.From(product, stockByVariant, documentStorage);
    }
}
