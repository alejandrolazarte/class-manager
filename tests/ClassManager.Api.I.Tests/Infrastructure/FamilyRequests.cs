using ClassManager.Core.UseCases.Authentication;
using ClassManager.Core.UseCases.Families;

namespace ClassManager.Api.I.Tests.Infrastructure;

public sealed record FamilyScenario(CoachScenario Coaches, Guid FamilyId, string Email, TokenResponse Tokens, HttpClient Family);

public static class FamilyRequests
{
    public const string FamilyFullName = "Ana Pérez";
    public const string FamilyPassword = "a long family passphrase";
    public const string WebAppAcceptFamilyInvitationUrl = "http://localhost:8081/accept-family-invitation?token=";

    public static string UniqueFamilyEmail() => $"family-{Guid.NewGuid():N}@example.com";

    public static Task<HttpResponseMessage> PostFamilyInvitationAsync(this HttpClient httpClient, Guid clientId, string email) =>
        httpClient.PostAsJsonAsync(
            $"{ApiRoutes.Clients}/{clientId}{ApiRoutes.AppInvitation}", new InviteFamilyRequest(email), ApiRequests.JsonOptions);

    public static string FamilyInvitationTokenSentTo(this RecordingEmailTransport emailTransport, string email)
    {
        var body = emailTransport.SentTo(email)[^1].TextBody;
        var link = body.Split('\n').Single(line => line.StartsWith(WebAppAcceptFamilyInvitationUrl, StringComparison.Ordinal));
        return Uri.UnescapeDataString(link[WebAppAcceptFamilyInvitationUrl.Length..]);
    }

    public static Task<HttpResponseMessage> PostAcceptFamilyInvitationAsync(this HttpClient httpClient, string token) =>
        httpClient.PostAsJsonAsync(
            ApiRoutes.Authentication + ApiRoutes.AcceptFamilyInvitation,
            new AcceptFamilyInvitationCommand(token, FamilyFullName, FamilyPassword),
            ApiRequests.JsonOptions);

    public static async Task<FamilyScenario> SeedFamilyScenarioAsync(this ApiFixture fixture) =>
        await fixture.InviteFamilyOfAsync(await fixture.SeedCoachScenarioAsync(), CoachScenario.CoachStudentFullName);

    public static async Task<FamilyScenario> InviteFamilyOfAsync(this ApiFixture fixture, CoachScenario coaches, string studentFullName)
    {
        var fees = await coaches.Business.HttpClient.GetMonthlyFeesAsync();
        var familyId = fees!.Clients.Single(client => client.StudentNames.Contains(studentFullName)).ClientId;
        var email = UniqueFamilyEmail();
        using (var invitation = await coaches.Business.HttpClient.PostFamilyInvitationAsync(familyId, email))
        {
            invitation.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        using var anonymous = fixture.ApiFactory.CreateClient();
        using var accepted = await anonymous.PostAcceptFamilyInvitationAsync(fixture.ApiFactory.EmailTransport.FamilyInvitationTokenSentTo(email));
        accepted.StatusCode.ShouldBe(HttpStatusCode.OK);
        var tokens = (await accepted.Content.ReadFromJsonAsync<TokenResponse>(ApiRequests.JsonOptions))!;

        return new FamilyScenario(coaches, familyId, email, tokens, fixture.CreateClientWithToken(tokens.AccessToken));
    }

    public static Task<FamilyHomeResponse?> GetFamilyHomeAsync(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<FamilyHomeResponse>(new Uri(ApiRoutes.Family, UriKind.Relative), ApiRequests.JsonOptions);
}
