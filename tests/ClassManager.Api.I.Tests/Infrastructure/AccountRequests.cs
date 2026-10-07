using ClassManager.Core.UseCases.Accounts;
using ClassManager.Core.UseCases.Authentication;
using ClassManager.Core.UseCases.Branches;

namespace ClassManager.Api.I.Tests.Infrastructure;

public sealed record OwnerAndStudent(StudentAppScenario Student, Guid OwnBusinessId, string OwnBusinessName, string Email, TokenResponse OwnerTokens);

public static class AccountRequests
{
    private static string SwitchRoute => ApiRoutes.Authentication + ApiRoutes.SwitchBranch;

    public static Task<List<AccountResponse>?> ListAccountsAsync(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<List<AccountResponse>>(new Uri(ApiRoutes.MyAccounts, UriKind.Relative), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PutMyProfileAsync(this HttpClient httpClient, string fullName, DateOnly? birthDate) =>
        httpClient.PutAsJsonAsync(ApiRoutes.MyAccount, new UpdateMyProfileCommand(fullName, birthDate), ApiRequests.JsonOptions);

    public static Task<MyAccountResponse?> GetMyAccountAsync(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<MyAccountResponse>(new Uri(ApiRoutes.MyAccount, UriKind.Relative), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PostSwitchAccountAsync(this HttpClient httpClient, string refreshToken, Guid businessId, string kind) =>
        httpClient.PostAsJsonAsync(SwitchRoute, new SwitchBranchCommand(refreshToken, businessId, kind), ApiRequests.JsonOptions);

    public static async Task<OwnerAndStudent> SeedOwnerWhoIsAlsoStudentAsync(this ApiFixture fixture)
    {
        var coaches = await fixture.SeedCoachScenarioAsync();
        var email = AuthenticationRequests.UniqueEmail();
        using var anonymous = fixture.ApiFactory.CreateClient();
        var signUp = await anonymous.SignUpAsync(AuthenticationRequests.SignUpCommand(email));
        using var ownerClient = fixture.CreateClientWithToken(signUp.AccessToken);
        var ownBusiness = (await ownerClient.ListBranchesAsync()).Single();

        var fees = await coaches.Business.HttpClient.GetMonthlyFeesAsync();
        var clientId = fees!.Clients.Single(client => client.StudentNames.Contains(CoachScenario.CoachStudentFullName)).ClientId;
        (await coaches.Business.HttpClient.PostStudentAppInvitationAsync(clientId, email)).EnsureSuccessStatusCode();
        (await anonymous.PostAcceptStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(email))).EnsureSuccessStatusCode();

        var ownerTokens = await anonymous.SignInAsync(email);
        var student = new StudentAppScenario(coaches, clientId, email, ownerTokens, fixture.CreateClientWithToken(ownerTokens.AccessToken));
        return new OwnerAndStudent(student, ownBusiness.BusinessId, ownBusiness.Name, email, ownerTokens);
    }
}
