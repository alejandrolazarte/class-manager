using ClassManager.Core.Abstractions.Persistence;
using ClassManager.Core.Common;

namespace ClassManager.Core.UseCases.Countries;

public sealed record ListCountriesQuery : IQuery;

public sealed record CountryResponse(string Code, string CallingCode, string CurrencyCode, IReadOnlyList<string> TimeZoneIds);

public sealed class ListCountriesUseCase(ICountryRepository countryRepository)
    : IUseCase<ListCountriesQuery, IReadOnlyList<CountryResponse>>
{
    public async Task<Result<IReadOnlyList<CountryResponse>>> ExecuteAsync(ListCountriesQuery command, CancellationToken cancellationToken)
    {
        var countries = await countryRepository.ListAsync(cancellationToken);

        return countries
            .Select(country => new CountryResponse(
                country.Code,
                country.CallingCode,
                country.CurrencyCode,
                [.. country.TimeZones.OrderBy(timeZone => timeZone.DisplayOrder).Select(timeZone => timeZone.TimeZoneId)]))
            .ToList();
    }
}
