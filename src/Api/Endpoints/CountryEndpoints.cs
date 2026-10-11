using ClassManager.Api.ErrorHandling;
using ClassManager.Core.UseCases.Countries;

namespace ClassManager.Api.Endpoints;

internal static class CountryEndpoints
{
    public static IEndpointRouteBuilder MapCountryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(ApiRoutes.Countries, ListCountriesAsync).AllowAnonymous();

        return endpoints;
    }

    private static async Task<IResult> ListCountriesAsync(
        IUseCase<ListCountriesQuery, IReadOnlyList<CountryResponse>> useCase,
        CancellationToken cancellationToken)
    {
        var result = await useCase.ExecuteAsync(new ListCountriesQuery(), cancellationToken);

        return result.ToOkResult();
    }
}
