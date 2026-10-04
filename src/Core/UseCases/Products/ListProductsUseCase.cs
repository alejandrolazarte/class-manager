using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Abstractions.Storage;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Products;

public sealed record ListProductsQuery(bool IncludeInactive) : IQuery;

public sealed class ListProductsUseCase(
    IProductRepository productRepository,
    IStockMovementRepository stockMovementRepository,
    IDocumentStorageService documentStorage)
    : IUseCase<ListProductsQuery, IReadOnlyList<ProductResponse>>
{
    public async Task<Result<IReadOnlyList<ProductResponse>>> ExecuteAsync(ListProductsQuery command, CancellationToken cancellationToken)
    {
        var products = await productRepository.ListAsync(command.IncludeInactive, cancellationToken);
        var variantIds = products.SelectMany(product => product.Variants).Select(variant => variant.Id).ToList();
        var stockByVariant = await stockMovementRepository.StockByVariantAsync(variantIds, cancellationToken);

        return Result.Success<IReadOnlyList<ProductResponse>>([.. products.Select(product => ProductResponse.From(product, stockByVariant, documentStorage))]);
    }
}
