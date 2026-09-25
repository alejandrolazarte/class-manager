using ClassManager.Core.UseCases.Businesses;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class SettingsRequests
{
    public const string MadridTimeZoneId = "Europe/Madrid";

    public static UpdateBusinessSettingsCommand SettingsOf(SeededBusiness business, string? timeZoneId = null, string? name = null) =>
        new(
            name ?? business.Business.Name,
            timeZoneId ?? business.Business.TimeZoneId,
            business.Business.CurrencyCode,
            business.Business.DefaultCountryCallingCode);

    public static Task<HttpResponseMessage> PutBusinessAsync(this HttpClient httpClient, UpdateBusinessSettingsCommand command) =>
        httpClient.PutAsJsonAsync(ApiRoutes.Business, command, ApiRequests.JsonOptions);
}
