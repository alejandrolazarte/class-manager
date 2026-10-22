using ClassManager.Core.UseCases.Businesses;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class SettingsRequests
{
    public const string MadridTimeZoneId = "Europe/Madrid";
    public const string EuroCurrencyCode = "EUR";

    public static UpdateBusinessSettingsCommand SettingsOf(
        SeededBusiness business,
        string? timeZoneId = null,
        string? name = null,
        string? currencyCode = null,
        string? locationName = null,
        string? locationMapsUrl = null) =>
        new(
            name ?? business.Business.Name,
            business.Business.CountryCode,
            timeZoneId ?? business.Business.TimeZoneId,
            currencyCode ?? business.Business.CurrencyCode,
            locationName,
            locationMapsUrl);

    public static UpdateBusinessSettingsCommand SpainSettingsOf(SeededBusiness business) =>
        new(business.Business.Name, ApiFixture.SpainCountryCode, MadridTimeZoneId, EuroCurrencyCode);

    public static Task<HttpResponseMessage> PutBusinessAsync(this HttpClient httpClient, UpdateBusinessSettingsCommand command) =>
        httpClient.PutAsJsonAsync(ApiRoutes.Business, command, ApiRequests.JsonOptions);

    public static Task<BusinessResponse?> GetCurrentBusinessAsync(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<BusinessResponse>(new Uri(ApiRoutes.Business, UriKind.Relative), ApiRequests.JsonOptions);
}
