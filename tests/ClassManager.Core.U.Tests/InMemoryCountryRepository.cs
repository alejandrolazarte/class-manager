using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Domain.Countries;

namespace ClassManager.Core.U.Tests;

internal sealed class InMemoryCountryRepository : ICountryRepository
{
    private readonly IReadOnlyList<Country> _countries = [TestData.Argentina(), TestData.Spain()];

    public Task<Country?> FindAsync(string? countryCode, CancellationToken cancellationToken) =>
        Task.FromResult(_countries.FirstOrDefault(country => country.Code == countryCode));

    public Task<IReadOnlyList<Country>> ListAsync(CancellationToken cancellationToken) => Task.FromResult(_countries);
}
