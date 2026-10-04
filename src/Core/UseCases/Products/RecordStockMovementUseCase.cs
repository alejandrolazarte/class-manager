using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.UseCases.Products;

public sealed record RecordStockMovementRequest(Guid? VariantId, StockMovementKind? Kind, int? Quantity, string? Note)
{
    public RecordStockMovementCommand ToCommand(Guid productId) => new(productId, VariantId, Kind, Quantity, Note);
}

public sealed record RecordStockMovementCommand(Guid ProductId, Guid? VariantId, StockMovementKind? Kind, int? Quantity, string? Note) : ICommand;

public sealed class RecordStockMovementUseCase(
    IProductRepository productRepository,
    IStockMovementRepository stockMovementRepository,
    IUnitOfWork unitOfWork,
    ICurrentMember currentMember,
    TimeProvider timeProvider,
    IDocumentStorageService documentStorage)
    : IUseCase<RecordStockMovementCommand, ProductResponse>
{
    public async Task<Result<ProductResponse>> ExecuteAsync(RecordStockMovementCommand command, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(command.ProductId, cancellationToken);
        if (product is null)
        {
            return ProductFailures.NotFound();
        }

        var variant = command.VariantId is { } variantId ? product.FindVariant(variantId) : null;
        if (variant is null || !variant.IsActive)
        {
            return ProductFailures.VariantNotFound();
        }

        var variantIds = product.Variants.Select(productVariant => productVariant.Id).ToList();
        var stockByVariant = await stockMovementRepository.StockByVariantAsync(variantIds, cancellationToken);
        var access = await currentMember.GetAccessAsync(cancellationToken);
        var movement = StockMovement.Load(
            product,
            variant,
            command.Kind,
            command.Quantity,
            command.Note,
            stockByVariant.GetValueOrDefault(variant.Id),
            access?.UserId,
            timeProvider.GetUtcNow());
        if (movement.IsFailure)
        {
            return movement.Error!;
        }

        stockMovementRepository.Add(movement.Value!);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return ProductResponse.From(product, await stockMovementRepository.StockByVariantAsync(variantIds, cancellationToken), documentStorage);
    }
}
