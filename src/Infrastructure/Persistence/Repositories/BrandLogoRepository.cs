namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class BrandLogoRepository(AppDbContext context) : IBrandLogoRepository
{
    public void Add(BrandLogo logo) => context.BrandLogos.Add(logo);

    public void Remove(BrandLogo logo) => context.BrandLogos.Remove(logo);

    public Task<BrandLogo?> GetAsync(CancellationToken cancellationToken) =>
        context.BrandLogos.AsNoTracking().FirstOrDefaultAsync(cancellationToken);

    public Task<BrandLogo?> GetForUpdateAsync(CancellationToken cancellationToken) =>
        context.BrandLogos.FirstOrDefaultAsync(cancellationToken);
}
