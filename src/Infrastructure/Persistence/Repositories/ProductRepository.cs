namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class ProductRepository(AppDbContext context) : IProductRepository
{
    public void Add(Product product) => context.Products.Add(product);

    public Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken) =>
        context.Products.AsNoTracking().Include(product => product.Variants)
            .FirstOrDefaultAsync(product => product.Id == productId, cancellationToken);

    public Task<Product?> GetForUpdateAsync(Guid productId, CancellationToken cancellationToken) =>
        context.Products.Include(product => product.Variants)
            .FirstOrDefaultAsync(product => product.Id == productId, cancellationToken);

    public Task<Product?> FindByNameAsync(string name, CancellationToken cancellationToken) =>
        context.Products.AsNoTracking().FirstOrDefaultAsync(product => product.Name == name, cancellationToken);

    public async Task<IReadOnlyList<Product>> ListAsync(bool includeInactive, CancellationToken cancellationToken) =>
        await context.Products.AsNoTracking()
            .Include(product => product.Variants)
            .Where(product => includeInactive || product.IsActive)
            .OrderByDescending(product => product.IsActive)
            .ThenBy(product => product.Name)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Product>> ListByVariantsAsync(IReadOnlyCollection<Guid> variantIds, CancellationToken cancellationToken)
    {
        if (variantIds.Count == 0)
        {
            return [];
        }

        return await context.Products.AsNoTracking()
            .Include(product => product.Variants)
            .Where(product => product.Variants.Any(variant => variantIds.Contains(variant.Id)))
            .ToListAsync(cancellationToken);
    }
}
