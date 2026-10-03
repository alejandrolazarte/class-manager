using ClassManager.Core.Abstractions.Persistence;
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
    IReadOnlyList<VariantChange>? Variants);

public sealed class CreateProductUseCase(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    IDocumentStorageService documentStorage)
    : IUseCase<CreateProductCommand, ProductResponse>
{
    private static readonly IReadOnlyList<VariantChange> SingleVariant = [new(null, string.Empty)];

    public async Task<Result<ProductResponse>> ExecuteAsync(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var variants = command.Variants is null or { Count: 0 } ? SingleVariant : command.Variants;
        var product = Product.Create(
            command.Name, command.Description, command.Price, command.StockMode, command.IsVisibleInApp, variants, timeProvider.GetUtcNow());
        if (product.IsFailure)
        {
            return product.Error!;
        }

        if (await productRepository.FindByNameAsync(product.Value!.Name, cancellationToken) is not null)
        {
            return ProductFailures.NameTaken();
        }

        productRepository.Add(product.Value);
        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (UniqueConstraintViolationException)
        {
            return ProductFailures.NameTaken();
        }

        return ProductResponse.From(product.Value, new Dictionary<Guid, int>(), documentStorage);
    }
}
