using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IBrandLogoRepository
{
    void Add(BrandLogo logo);

    void Remove(BrandLogo logo);

    Task<BrandLogo?> GetAsync(CancellationToken cancellationToken);

    Task<BrandLogo?> GetForUpdateAsync(CancellationToken cancellationToken);
}
