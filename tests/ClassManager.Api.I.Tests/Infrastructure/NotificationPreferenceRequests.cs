using ClassManager.Core.Domain.Notifications;
using ClassManager.Core.UseCases.Notifications;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class NotificationPreferenceRequests
{
    public static Task<NotificationPreferencesResponse?> GetMyNotificationPreferencesAsync(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<NotificationPreferencesResponse>(
            new Uri(ApiRoutes.MyNotificationPreferences, UriKind.Relative), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PutMyNotificationPreferenceAsync(
        this HttpClient httpClient, NotificationCategory category, NotificationChannel channel, bool isEnabled) =>
        httpClient.PutAsJsonAsync(
            ApiRoutes.MyNotificationPreferences, new UpdateMyNotificationPreferenceCommand(category, channel, isEnabled), ApiRequests.JsonOptions);

    public static async Task TurnOffAsync(this HttpClient httpClient, NotificationCategory category, NotificationChannel channel) =>
        (await httpClient.PutMyNotificationPreferenceAsync(category, channel, isEnabled: false)).EnsureSuccessStatusCode();

    public static bool IsEnabled(this NotificationPreferencesResponse response, NotificationCategory category, NotificationChannel channel) =>
        response.Preferences.Single(preference => preference.Category == category).Channels.Single(candidate => candidate.Channel == channel).IsEnabled;
}
