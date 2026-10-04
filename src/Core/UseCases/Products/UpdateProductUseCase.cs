using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.UseCases.Products;

public sealed record UpdateProductRequest(
    string? Name,
    string? Description,
    decimal? Price,
    StockMode? StockMode,
    bool IsVisibleInApp,
    IReadOnlyList<VariantChange>? Variants)
{
    public UpdateProductCommand ToCommand(Guid productId) => new(productId, Name, Description, Price, StockMode, IsVisibleInApp, Variants);
}

public sealed record UpdateProductCommand(
    Guid ProductId,
    string? Name,
    string? Description,
    decimal? Price,
    StockMode? StockMode,
    bool IsVisibleInApp,
    IReadOnlyList<VariantChange>? Variants) : ICommand;

public sealed class UpdateProductUseCase(
    IProductRepository productRepository,
    IStockMovementRepository stockMovementRepository,
    IUnitOfWork unitOfWork,
    IDocumentStorageService documentStorage)
    : IUseCase<UpdateProductCommand, ProductResponse>
{
    public async Task<Result<ProductResponse>> ExecuteAsync(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetForUpdateAsync(command.ProductId, cancellationToken);
        if (product is null)
        {
            return ProductFailures.NotFound();
        }

        var update = product.Update(command.Name, command.Description, command.Price, command.StockMode, command.IsVisibleInApp, command.Variants);
        if (update.IsFailure)
        {
            return update.Error!;
        }

        var sameName = await productRepository.FindByNameAsync(product.Name, cancellationToken);
        if (sameName is not null && sameName.Id != product.Id)
        {
            return ProductFailures.NameTaken();
        }

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            return ProductFailures.NameTaken();
        }

        var stockByVariant = await stockMovementRepository.StockByVariantAsync([.. product.Variants.Select(variant => variant.Id)], cancellationToken);
        return ProductResponse.From(product, stockByVariant, documentStorage);
    }
}
