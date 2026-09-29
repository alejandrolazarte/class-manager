using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Products;

public sealed record SetProductActiveCommand(Guid ProductId, bool IsActive);

public sealed class SetProductActiveUseCase(
    IProductRepository productRepository,
    IStockMovementRepository stockMovementRepository,
    IUnitOfWork unitOfWork)
    : IUseCase<SetProductActiveCommand, ProductResponse>
{
    public async Task<Result<ProductResponse>> ExecuteAsync(SetProductActiveCommand command, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetForUpdateAsync(command.ProductId, cancellationToken);
        if (product is null)
        {
            return ProductFailures.NotFound();
        }

        if (command.IsActive)
        {
            product.Activate();
        }
        else
        {
            product.Deactivate();
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var stockByVariant = await stockMovementRepository.StockByVariantAsync([.. product.Variants.Select(variant => variant.Id)], cancellationToken);
        return ProductResponse.From(product, stockByVariant);
    }
}
