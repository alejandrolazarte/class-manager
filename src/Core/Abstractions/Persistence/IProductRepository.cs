using ClassManager.Core.Domain.Products;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IProductRepository
{
    void Add(Product product);

    Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken);

    Task<Product?> GetForUpdateAsync(Guid productId, CancellationToken cancellationToken);

    Task<Product?> FindByNameAsync(string name, CancellationToken cancellationToken);

    Task<IReadOnlyList<Product>> ListAsync(bool includeInactive, CancellationToken cancellationToken);

    Task<IReadOnlyList<Product>> ListByVariantsAsync(IReadOnlyCollection<Guid> variantIds, CancellationToken cancellationToken);
}
