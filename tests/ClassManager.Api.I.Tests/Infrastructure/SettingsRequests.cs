using ClassManager.Core.UseCases.Businesses;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class SettingsRequests
{
    public const string MadridTimeZoneId = "Europe/Madrid";

    public static UpdateBusinessSettingsCommand SettingsOf(
        SeededBusiness business, string? timeZoneId = null, string? name = null, bool? noticedAbsencesKeepStreak = null) =>
        new(
            name ?? business.Business.Name,
            timeZoneId ?? business.Business.TimeZoneId,
            business.Business.CurrencyCode,
            business.Business.DefaultCountryCallingCode,
            noticedAbsencesKeepStreak);

    public static Task<HttpResponseMessage> PutBusinessAsync(this HttpClient httpClient, UpdateBusinessSettingsCommand command) =>
        httpClient.PutAsJsonAsync(ApiRoutes.Business, command, ApiRequests.JsonOptions);

    public static Task<BusinessResponse?> GetCurrentBusinessAsync(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<BusinessResponse>(new Uri(ApiRoutes.Business, UriKind.Relative), ApiRequests.JsonOptions);
}
