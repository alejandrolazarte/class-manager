using ClassManager.Core.Domain.Countries;

namespace ClassManager.Core.Abstractions.Persistence;

public interface ICountryRepository
{
    Task<Country?> FindAsync(string? countryCode, CancellationToken cancellationToken);

    Task<IReadOnlyList<Country>> ListAsync(CancellationToken cancellationToken);
}
