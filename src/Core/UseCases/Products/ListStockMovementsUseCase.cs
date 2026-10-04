using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.UseCases.Products;

public sealed record ListStockMovementsQuery(Guid ProductId) : IQuery;

public sealed record StockMovementResponse(
    Guid Id,
    Guid VariantId,
    string VariantName,
    StockMovementKind Kind,
    int Quantity,
    Guid? OrderId,
    string? Note,
    DateTimeOffset CreatedAt);

public sealed class ListStockMovementsUseCase(IProductRepository productRepository, IStockMovementRepository stockMovementRepository)
    : IUseCase<ListStockMovementsQuery, IReadOnlyList<StockMovementResponse>>
{
    private const int MovementLimit = 100;

    public async Task<Result<IReadOnlyList<StockMovementResponse>>> ExecuteAsync(ListStockMovementsQuery command, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(command.ProductId, cancellationToken);
        if (product is null)
        {
            return ProductFailures.NotFound();
        }

        var variantNames = product.Variants.ToDictionary(variant => variant.Id, variant => variant.Name);
        var movements = await stockMovementRepository.ListByProductAsync(product.Id, MovementLimit, cancellationToken);
        return Result.Success<IReadOnlyList<StockMovementResponse>>(
        [
            .. movements.Select(movement => new StockMovementResponse(
                movement.Id,
                movement.ProductVariantId,
                variantNames.GetValueOrDefault(movement.ProductVariantId, string.Empty),
                movement.Kind,
                movement.Quantity,
                movement.OrderId,
                movement.Note,
                movement.CreatedAt)),
        ]);
    }
}
