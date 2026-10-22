namespace ClassManager.Backoffice.Core.Brands;

public interface IBrandDirectory
{
    Task<IReadOnlyList<BrandListing>> ListAsync(CancellationToken cancellationToken);

    Task<BrandRecord?> FindAsync(long brandId, CancellationToken cancellationToken);

    Task<IReadOnlyList<ExpiringAddOn>> ListAddOnsExpiringBetweenAsync(DateOnly from, DateOnly until, CancellationToken cancellationToken);
}
