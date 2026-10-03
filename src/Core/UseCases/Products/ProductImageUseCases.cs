using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Images;
using ClassManager.Core.UseCases.Images;

namespace ClassManager.Core.UseCases.Products;

public sealed record SetProductImageCommand(Guid ProductId, byte[] Content);

public sealed record RemoveProductImageCommand(Guid ProductId);

public sealed class SetProductImageUseCase(
    IProductRepository productRepository,
    IStockMovementRepository stockMovementRepository,
    ICatalogImageService catalogImageService,
    IUnitOfWork unitOfWork)
    : IUseCase<SetProductImageCommand, ProductResponse>
{
    public async Task<Result<ProductResponse>> ExecuteAsync(SetProductImageCommand command, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetForUpdateAsync(command.ProductId, cancellationToken);
        if (product is null)
        {
            return ProductFailures.NotFound();
        }

        var change = await CatalogImageChanges.ReplaceAsync(
            product, CatalogImageOwner.Product, command.Content, catalogImageService, unitOfWork, cancellationToken);
        if (change.IsFailure)
        {
            return change.Error!;
        }

        var stockByVariant = await stockMovementRepository.StockByVariantAsync([.. product.Variants.Select(variant => variant.Id)], cancellationToken);
        return ProductResponse.From(product, stockByVariant);
    }
}

public sealed class RemoveProductImageUseCase(
    IProductRepository productRepository,
    IStockMovementRepository stockMovementRepository,
    ICatalogImageService catalogImageService,
    IUnitOfWork unitOfWork)
    : IUseCase<RemoveProductImageCommand, ProductResponse>
{
    public async Task<Result<ProductResponse>> ExecuteAsync(RemoveProductImageCommand command, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetForUpdateAsync(command.ProductId, cancellationToken);
        if (product is null)
        {
            return ProductFailures.NotFound();
        }

        await CatalogImageChanges.RemoveAsync(product, catalogImageService, unitOfWork, cancellationToken);

        var stockByVariant = await stockMovementRepository.StockByVariantAsync([.. product.Variants.Select(variant => variant.Id)], cancellationToken);
        return ProductResponse.From(product, stockByVariant);
    }
}
