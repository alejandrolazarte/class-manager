namespace ClassManager.Infrastructure.Persistence.Repositories;

internal sealed class CountryRepository(AppDbContext context) : ICountryRepository
{
    public Task<Country?> FindAsync(string? countryCode, CancellationToken cancellationToken) =>
        context.Countries.FirstOrDefaultAsync(country => country.Code == countryCode, cancellationToken);

    public async Task<IReadOnlyList<Country>> ListAsync(CancellationToken cancellationToken) =>
        await context.Countries.AsNoTracking().OrderBy(country => country.DisplayOrder).ToListAsync(cancellationToken);
}
