using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Common;
using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.UseCases.Products;

public sealed record CreateProductCommand(
    string? Name,
    string? Description,
    decimal? Price,
    StockMode? StockMode,
    bool IsVisibleInApp,
    IReadOnlyList<VariantChange>? Variants) : ICommand;

public sealed class CreateProductUseCase(
    IProductRepository productRepository,
    IStockMovementRepository stockMovementRepository,
    IUnitOfWork unitOfWork,
    ICurrentMember currentMember,
    TimeProvider timeProvider,
    IDocumentStorageService documentStorage)
    : IUseCase<CreateProductCommand, ProductResponse>
{
    private static readonly IReadOnlyList<VariantChange> SingleVariant = [new(null, string.Empty)];

    public async Task<Result<ProductResponse>> ExecuteAsync(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var variants = command.Variants is null or { Count: 0 } ? SingleVariant : command.Variants;
        var now = timeProvider.GetUtcNow();
        var product = Product.Create(
            command.Name, command.Description, command.Price, command.StockMode, command.IsVisibleInApp, variants, now);
        if (product.IsFailure)
        {
            return product.Error!;
        }

        var access = await currentMember.GetAccessAsync(cancellationToken);
        var initialStock = StockMovement.InitialStock(product.Value!, variants, access?.AccountId, now);
        if (initialStock.IsFailure)
        {
            return initialStock.Error!;
        }

        if (await productRepository.FindByNameAsync(product.Value!.Name, cancellationToken) is not null)
        {
            return ProductFailures.NameTaken();
        }

        productRepository.Add(product.Value);
        foreach (var movement in initialStock.Value!)
        {
            stockMovementRepository.Add(movement);
        }

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            return ProductFailures.NameTaken();
        }

        var stockByVariant = initialStock.Value!.ToDictionary(movement => movement.ProductVariantId, movement => movement.Quantity);
        return ProductResponse.From(product.Value, stockByVariant, documentStorage);
    }
}
