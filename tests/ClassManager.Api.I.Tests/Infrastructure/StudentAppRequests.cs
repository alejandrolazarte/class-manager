using ClassManager.Core.UseCases.Authentication;
using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Infrastructure;

public sealed record StudentAppScenario(CoachScenario Coaches, Guid ClientId, string Email, TokenResponse Tokens, HttpClient Student);

public static class StudentAppRequests
{
    public const string StudentAppFullName = "Ana Pérez";
    public const string StudentAppPassword = "a long student passphrase";
    public const string WebAppAcceptStudentAppInvitationUrl = "http://localhost:8081/accept-student-invitation?token=";

    public static string UniqueStudentEmail() => $"student-{Guid.NewGuid():N}@example.com";

    public static Task<HttpResponseMessage> PostStudentAppInvitationAsync(this HttpClient httpClient, Guid clientId, string email) =>
        httpClient.PostAsJsonAsync(
            $"{ApiRoutes.Clients}/{clientId}{ApiRoutes.AppInvitation}", new InviteStudentAppRequest(email), ApiRequests.JsonOptions);

    public static string StudentAppInvitationTokenSentTo(this RecordingEmailTransport emailTransport, string email)
    {
        var body = emailTransport.SentTo(email)[^1].TextBody;
        var link = body.Split('\n').Single(line => line.StartsWith(WebAppAcceptStudentAppInvitationUrl, StringComparison.Ordinal));
        return Uri.UnescapeDataString(link[WebAppAcceptStudentAppInvitationUrl.Length..]);
    }

    public static Task<HttpResponseMessage> PostAcceptStudentAppInvitationAsync(this HttpClient httpClient, string token) =>
        httpClient.PostAsJsonAsync(
            ApiRoutes.Authentication + ApiRoutes.AcceptStudentAppInvitation,
            new AcceptStudentAppInvitationCommand(token, StudentAppFullName, StudentAppPassword),
            ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PostCheckStudentAppInvitationAsync(this HttpClient httpClient, string token) =>
        httpClient.PostAsJsonAsync(
            ApiRoutes.Authentication + ApiRoutes.CheckStudentAppInvitation,
            new CheckStudentAppInvitationCommand(token),
            ApiRequests.JsonOptions);

    public static async Task<StudentAppScenario> SeedStudentAppScenarioAsync(this ApiFixture fixture) =>
        await fixture.InviteStudentAppOfAsync(await fixture.SeedCoachScenarioAsync(), CoachScenario.CoachStudentFullName);

    public static async Task<StudentAppScenario> InviteStudentAppOfAsync(this ApiFixture fixture, CoachScenario coaches, string studentFullName)
    {
        var fees = await coaches.Business.HttpClient.GetMonthlyFeesAsync();
        var clientId = fees!.Clients.Single(client => client.StudentNames.Contains(studentFullName)).ClientId;
        var email = UniqueStudentEmail();
        using (var invitation = await coaches.Business.HttpClient.PostStudentAppInvitationAsync(clientId, email))
        {
            invitation.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        using var anonymous = fixture.ApiFactory.CreateClient();
        using var accepted = await anonymous.PostAcceptStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(email));
        accepted.StatusCode.ShouldBe(HttpStatusCode.OK);
        var tokens = (await accepted.Content.ReadFromJsonAsync<TokenResponse>(ApiRequests.JsonOptions))!;

        return new StudentAppScenario(coaches, clientId, email, tokens, fixture.CreateClientWithToken(tokens.AccessToken));
    }

    public static Task<StudentAppHomeResponse?> GetStudentAppHomeAsync(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<StudentAppHomeResponse>(new Uri(ApiRoutes.StudentApp, UriKind.Relative), ApiRequests.JsonOptions);
}
