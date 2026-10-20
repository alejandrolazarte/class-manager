namespace ClassManager.Backoffice.Core.Brands;

public interface IBrandDirectory
{
    Task<IReadOnlyList<BrandListing>> ListAsync(CancellationToken cancellationToken);

    Task<BrandRecord?> FindAsync(Guid brandId, CancellationToken cancellationToken);
}
