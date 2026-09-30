using ClassManager.Core.UseCases.Announcements;
using ClassManager.Core.UseCases.Families;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class NewsRequests
{
    public const string AnnouncementTitle = "Lunes 12 cerrado";
    public const string AnnouncementBody = "Feriado. Las clases se recuperan durante la semana.";

    public static Task<HttpResponseMessage> PostAnnouncementAsync(
        this HttpClient httpClient, string title = AnnouncementTitle, string? body = AnnouncementBody) =>
        httpClient.PostAsJsonAsync(ApiRoutes.Announcements, new CreateAnnouncementCommand(title, body), ApiRequests.JsonOptions);

    public static async Task<AnnouncementResponse> PublishAnnouncementAsync(this HttpClient httpClient)
    {
        using var response = await httpClient.PostAnnouncementAsync();
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<AnnouncementResponse>(ApiRequests.JsonOptions))!;
    }

    public static Task<List<AnnouncementResponse>?> ListAnnouncementsAsync(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<List<AnnouncementResponse>>(new Uri(ApiRoutes.Announcements, UriKind.Relative), ApiRequests.JsonOptions);

    public static Task<FamilyNewsResponse?> GetFamilyNewsAsync(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<FamilyNewsResponse>(
            new Uri(ApiRoutes.Family + ApiRoutes.FamilyNews, UriKind.Relative), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PutFamilyNewsSeenAsync(this HttpClient httpClient) =>
        httpClient.PutAsJsonAsync(ApiRoutes.Family + ApiRoutes.FamilyNews + ApiRoutes.Seen, new { }, ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PutSessionCancellationAsync(
        this HttpClient httpClient, Guid classGroupId, DateOnly sessionDate, string reason) =>
        httpClient.PutAsJsonAsync(
            $"{SessionRequests.SessionPath(classGroupId, sessionDate)}{ApiRoutes.Cancellation}",
            new CancelSessionRequest(reason),
            ApiRequests.JsonOptions);
}
