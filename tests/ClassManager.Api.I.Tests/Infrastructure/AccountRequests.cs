using ClassManager.Core.UseCases.Authentication;
using ClassManager.Core.UseCases.Branches;

namespace ClassManager.Api.I.Tests.Infrastructure;

public sealed record OwnerAndFamily(FamilyScenario Family, Guid OwnBusinessId, string OwnBusinessName, string Email, TokenResponse OwnerTokens);

public static class AccountRequests
{
    private static string SwitchRoute => ApiRoutes.Authentication + ApiRoutes.SwitchBranch;

    public static Task<List<AccountResponse>?> ListAccountsAsync(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<List<AccountResponse>>(new Uri(ApiRoutes.MyAccounts, UriKind.Relative), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PostSwitchAccountAsync(this HttpClient httpClient, string refreshToken, Guid businessId, string kind) =>
        httpClient.PostAsJsonAsync(SwitchRoute, new SwitchBranchCommand(refreshToken, businessId, kind), ApiRequests.JsonOptions);

    public static async Task<OwnerAndFamily> SeedOwnerWhoIsAlsoFamilyAsync(this ApiFixture fixture)
    {
        var coaches = await fixture.SeedCoachScenarioAsync();
        var email = AuthenticationRequests.UniqueEmail();
        using var anonymous = fixture.ApiFactory.CreateClient();
        var signUp = await anonymous.SignUpAsync(AuthenticationRequests.SignUpCommand(email));
        using var ownerClient = fixture.CreateClientWithToken(signUp.AccessToken);
        var ownBusiness = (await ownerClient.ListBranchesAsync()).Single();

        var fees = await coaches.Business.HttpClient.GetMonthlyFeesAsync();
        var familyId = fees!.Clients.Single(client => client.StudentNames.Contains(CoachScenario.CoachStudentFullName)).ClientId;
        (await coaches.Business.HttpClient.PostFamilyInvitationAsync(familyId, email)).EnsureSuccessStatusCode();
        (await anonymous.PostAcceptFamilyInvitationAsync(fixture.ApiFactory.EmailTransport.FamilyInvitationTokenSentTo(email))).EnsureSuccessStatusCode();

        var ownerTokens = await anonymous.SignInAsync(email);
        var family = new FamilyScenario(coaches, familyId, email, ownerTokens, fixture.CreateClientWithToken(ownerTokens.AccessToken));
        return new OwnerAndFamily(family, ownBusiness.BusinessId, ownBusiness.Name, email, ownerTokens);
    }
}
