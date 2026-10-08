using ClassManager.Core.UseCases.Authentication;
using ClassManager.Core.UseCases.Clients;
using ClassManager.Core.UseCases.StudentApp;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Infrastructure;

public sealed record StudentAppScenario(InstructorScenario Instructors, Guid ClientId, string Email, TokenResponse Tokens, HttpClient Student);

public static class StudentAppRequests
{
    public const string StudentAppFullName = "Ana Pérez";
    public const string ChildFullName = "Tomás Pérez";
    public static readonly DateOnly ChildBirthDate = new(2012, 5, 1);
    public static readonly DateOnly ElevenYearsOld = new(2015, 3, 10);
    public const string StudentAppPassword = "a long student passphrase";
    public const string WebAppAcceptStudentAppInvitationUrl = "http://localhost:8081/accept-student-invitation?token=";
    public const string WebAppAuthorizeStudentAppUrl = "http://localhost:8081/authorize-student-app?token=";

    public static string UniqueStudentEmail() => $"student-{Guid.NewGuid():N}@example.com";

    public static Task<HttpResponseMessage> PostStudentAppInvitationAsync(this HttpClient httpClient, Guid clientId, string email) =>
        httpClient.PostAsJsonAsync(
            $"{ApiRoutes.Clients}/{clientId}{ApiRoutes.AppInvitation}", new InviteStudentAppRequest(email), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PostChildAppInvitationAsync(this HttpClient httpClient, Guid clientId, Guid studentId, string email, DateOnly? birthDate) =>
        httpClient.PostAsJsonAsync(
            $"{ApiRoutes.Clients}/{clientId}{ApiRoutes.AppInvitation}", new InviteStudentAppRequest(email, studentId, birthDate), ApiRequests.JsonOptions);

    public static async Task<ClientDetailsResponse> RegisterFamilyAsync(this HttpClient httpClient, string clientEmail)
    {
        using var response = await httpClient.PostAsJsonAsync(
            ApiRoutes.Clients,
            new RegisterClientCommand(ApiRequests.ClientFullName, "11 4321-8765", clientEmail, null, [new NewStudent(ChildFullName, null, null)]),
            ApiRequests.JsonOptions);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ClientDetailsResponse>(ApiRequests.JsonOptions))!;
    }

    public static string StudentAppInvitationTokenSentTo(this RecordingEmailTransport emailTransport, string email)
    {
        var body = emailTransport.SentTo(email)[^1].TextBody;
        var link = body.Split('\n').Single(line => line.StartsWith(WebAppAcceptStudentAppInvitationUrl, StringComparison.Ordinal));
        return Uri.UnescapeDataString(link[WebAppAcceptStudentAppInvitationUrl.Length..]);
    }

    public static string GuardianConsentTokenSentTo(this RecordingEmailTransport emailTransport, string email)
    {
        var body = emailTransport.SentTo(email)[^1].TextBody;
        var link = body.Split('\n').Single(line => line.StartsWith(WebAppAuthorizeStudentAppUrl, StringComparison.Ordinal));
        return Uri.UnescapeDataString(link[WebAppAuthorizeStudentAppUrl.Length..]);
    }

    public static async Task<HttpClient> SignInClientOfFamilyAsync(this ApiFixture fixture, HttpClient team, Guid clientId, string clientEmail)
    {
        (await team.PostStudentAppInvitationAsync(clientId, clientEmail)).EnsureSuccessStatusCode();
        using var anonymous = fixture.ApiFactory.CreateClient();
        using var accepted = await anonymous.PostAcceptStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(clientEmail));
        accepted.EnsureSuccessStatusCode();
        var tokens = (await accepted.Content.ReadFromJsonAsync<TokenResponse>(ApiRequests.JsonOptions))!;
        return fixture.CreateClientWithToken(tokens.AccessToken);
    }

    public static Task<HttpResponseMessage> PostGuardianConsentInAppAsync(this HttpClient httpClient, Guid invitationId) =>
        httpClient.PostAsync(new Uri($"{ApiRoutes.StudentApp}/guardian-consents/{invitationId}{ApiRoutes.Authorization}", UriKind.Relative), null);

    public static Task<HttpResponseMessage> PostGiveGuardianConsentAsync(this HttpClient httpClient, string token) =>
        httpClient.PostAsJsonAsync(
            ApiRoutes.Authentication + ApiRoutes.GiveGuardianConsent, new GiveGuardianConsentCommand(token), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PostRefuseGuardianConsentAsync(this HttpClient httpClient, string token) =>
        httpClient.PostAsJsonAsync(
            ApiRoutes.Authentication + ApiRoutes.RefuseGuardianConsent, new RefuseGuardianConsentCommand(token), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PostAcceptStudentAppInvitationAsync(this HttpClient httpClient, string token, DateOnly birthDate, string password = StudentAppPassword) =>
        httpClient.PostAsJsonAsync(
            ApiRoutes.Authentication + ApiRoutes.AcceptStudentAppInvitation,
            new AcceptStudentAppInvitationCommand(token, null, password, birthDate),
            ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PostAcceptStudentAppInvitationAsync(this HttpClient httpClient, string token, string password = StudentAppPassword) =>
        httpClient.PostAsJsonAsync(
            ApiRoutes.Authentication + ApiRoutes.AcceptStudentAppInvitation,
            new AcceptStudentAppInvitationCommand(token, StudentAppFullName, password, AuthenticationRequests.AdultBirthDate),
            ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PostDeclineStudentAppInvitationAsync(this HttpClient httpClient, string token) =>
        httpClient.PostAsJsonAsync(
            ApiRoutes.Authentication + ApiRoutes.DeclineStudentAppInvitation,
            new DeclineStudentAppInvitationCommand(token),
            ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PostCheckStudentAppInvitationAsync(this HttpClient httpClient, string token) =>
        httpClient.PostAsJsonAsync(
            ApiRoutes.Authentication + ApiRoutes.CheckStudentAppInvitation,
            new CheckStudentAppInvitationCommand(token),
            ApiRequests.JsonOptions);

    public static async Task<StudentAppScenario> SeedStudentAppScenarioAsync(this ApiFixture fixture) =>
        await fixture.InviteStudentAppOfAsync(await fixture.SeedInstructorScenarioAsync(), InstructorScenario.InstructorStudentFullName);

    public static async Task<StudentAppScenario> InviteStudentAppOfAsync(this ApiFixture fixture, InstructorScenario instructors, string studentFullName)
    {
        var fees = await instructors.Business.HttpClient.GetMonthlyFeesAsync();
        var clientId = fees!.Clients.Single(client => client.StudentNames.Contains(studentFullName)).ClientId;
        var email = UniqueStudentEmail();
        using (var invitation = await instructors.Business.HttpClient.PostStudentAppInvitationAsync(clientId, email))
        {
            invitation.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        using var anonymous = fixture.ApiFactory.CreateClient();
        using var accepted = await anonymous.PostAcceptStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(email));
        accepted.StatusCode.ShouldBe(HttpStatusCode.OK);
        var tokens = (await accepted.Content.ReadFromJsonAsync<TokenResponse>(ApiRequests.JsonOptions))!;

        return new StudentAppScenario(instructors, clientId, email, tokens, fixture.CreateClientWithToken(tokens.AccessToken));
    }

    public static Task<StudentAppHomeResponse?> GetStudentAppHomeAsync(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<StudentAppHomeResponse>(new Uri(ApiRoutes.StudentApp, UriKind.Relative), ApiRequests.JsonOptions);
}
