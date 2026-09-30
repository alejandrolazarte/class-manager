using ClassManager.Core.UseCases.TeamNotifications;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class TeamNotificationRequests
{
    private static string NotificationsPath => ApiRoutes.Team + ApiRoutes.Notifications;

    public static Task<TeamNotificationsResponse?> GetTeamNotificationsAsync(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<TeamNotificationsResponse>(new Uri(NotificationsPath, UriKind.Relative), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PutTeamNotificationsSeenAsync(this HttpClient httpClient) =>
        httpClient.PutAsync(new Uri(NotificationsPath + ApiRoutes.Seen, UriKind.Relative), content: null);

    public static async Task<string> SubscribeToTeamPushAsync(this HttpClient httpClient)
    {
        var subscription = PushRequests.NewSubscription();
        var command = new SaveTeamPushSubscriptionCommand(subscription.Endpoint, subscription.P256dh, subscription.Auth);
        (await httpClient.PutAsJsonAsync(ApiRoutes.Team + ApiRoutes.PushSubscription, command, ApiRequests.JsonOptions)).EnsureSuccessStatusCode();
        return command.Endpoint!;
    }
}
