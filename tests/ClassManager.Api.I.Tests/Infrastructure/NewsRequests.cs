using ClassManager.Core.UseCases.Announcements;
using ClassManager.Core.UseCases.Sessions;
using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class NewsRequests
{
    public const string AnnouncementTitle = "Lunes 12 cerrado";
    public const string AnnouncementBody = "Feriado. Las clases se recuperan durante la semana.";

    public static Task<HttpResponseMessage> PostAnnouncementAsync(
        this HttpClient httpClient, string title = AnnouncementTitle, string? body = AnnouncementBody) =>
        httpClient.PostAsJsonAsync(ApiRoutes.Announcements, new CreateAnnouncementCommand(title, body), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PostAnnouncementAsync(this HttpClient httpClient, CreateAnnouncementCommand command) =>
        httpClient.PostAsJsonAsync(ApiRoutes.Announcements, command, ApiRequests.JsonOptions);

    public static Task<AnnouncementResponse> PublishAnnouncementAsync(this HttpClient httpClient) =>
        httpClient.PublishAnnouncementAsync(new CreateAnnouncementCommand(AnnouncementTitle, AnnouncementBody));

    public static async Task<AnnouncementResponse> PublishAnnouncementAsync(this HttpClient httpClient, CreateAnnouncementCommand command)
    {
        using var response = await httpClient.PostAnnouncementAsync(command);
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<AnnouncementResponse>(ApiRequests.JsonOptions))!;
    }

    public static Task<HttpResponseMessage> PinAnnouncementAsync(this HttpClient httpClient, Guid announcementId) =>
        httpClient.PostAsync(new Uri($"{ApiRoutes.Announcements}/{announcementId}{ApiRoutes.Pin}", UriKind.Relative), null);

    public static Task<List<AnnouncementRecipientResponse>?> GetAnnouncementRecipientsAsync(this HttpClient httpClient, Guid announcementId) =>
        httpClient.GetFromJsonAsync<List<AnnouncementRecipientResponse>>(
            new Uri($"{ApiRoutes.Announcements}/{announcementId}{ApiRoutes.Recipients}", UriKind.Relative), ApiRequests.JsonOptions);

    public static async Task<AnnouncementReminderResponse> RemindAnnouncementAsync(this HttpClient httpClient, Guid announcementId)
    {
        using var response = await httpClient.PostAsync(new Uri($"{ApiRoutes.Announcements}/{announcementId}{ApiRoutes.Reminder}", UriKind.Relative), null);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        return (await response.Content.ReadFromJsonAsync<AnnouncementReminderResponse>(ApiRequests.JsonOptions))!;
    }

    public static Task<HttpResponseMessage> PutAnnouncementReplyAsync(this HttpClient httpClient, Guid announcementId, bool isGoing) =>
        httpClient.PutAsJsonAsync(
            $"{ApiRoutes.StudentApp}{ApiRoutes.StudentAppNewsAnnouncements}/{announcementId}{ApiRoutes.Reply}",
            new ReplyToAnnouncementRequest(isGoing),
            ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> UnpinAnnouncementAsync(this HttpClient httpClient, Guid announcementId) =>
        httpClient.DeleteAsync(new Uri($"{ApiRoutes.Announcements}/{announcementId}{ApiRoutes.Pin}", UriKind.Relative));

    public static Task<List<AnnouncementResponse>?> ListAnnouncementsAsync(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<List<AnnouncementResponse>>(new Uri(ApiRoutes.Announcements, UriKind.Relative), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> DeleteAnnouncementAsync(this HttpClient httpClient, Guid announcementId) =>
        httpClient.DeleteAsync(new Uri($"{ApiRoutes.Announcements}/{announcementId}", UriKind.Relative));

    public static Task<HttpResponseMessage> RestoreAnnouncementAsync(this HttpClient httpClient, Guid announcementId) =>
        httpClient.PostAsync(new Uri($"{ApiRoutes.Announcements}/{announcementId}{ApiRoutes.Restore}", UriKind.Relative), null);

    public static Task<StudentAppNewsResponse?> GetStudentAppNewsAsync(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<StudentAppNewsResponse>(
            new Uri(ApiRoutes.StudentApp + ApiRoutes.StudentAppNews, UriKind.Relative), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PutStudentAppNewsSeenAsync(this HttpClient httpClient) =>
        httpClient.PutAsJsonAsync(ApiRoutes.StudentApp + ApiRoutes.StudentAppNews + ApiRoutes.Seen, new { }, ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PutSessionCancellationAsync(
        this HttpClient httpClient, Guid classGroupId, DateOnly sessionDate, string reason) =>
        httpClient.PutAsJsonAsync(
            $"{SessionRequests.SessionPath(classGroupId, sessionDate)}{ApiRoutes.Cancellation}",
            new CancelSessionRequest(reason),
            ApiRequests.JsonOptions);
}
