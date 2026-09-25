using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Core.Abstractions.Persistence;

public interface IBusinessRepository
{
    void Add(Business business);

    Task<Business?> GetCurrentAsync(CancellationToken cancellationToken);

    Task<Business?> GetCurrentForUpdateAsync(CancellationToken cancellationToken);

    Task<bool> IsSlugTakenAsync(string slug, CancellationToken cancellationToken);
}
