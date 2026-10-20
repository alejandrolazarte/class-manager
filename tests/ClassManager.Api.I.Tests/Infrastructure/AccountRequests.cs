using ClassManager.Core.Abstractions.Security;
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

    public static Task<HttpResponseMessage> PostAppFeedbackAsync(this HttpClient httpClient, string message) =>
        httpClient.PostAsJsonAsync(ApiRoutes.MyAppFeedback, new SendAppFeedbackCommand(message, "web", "1.0.0"), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PutMyLanguageAsync(this HttpClient httpClient, string preferredLanguage) =>
        httpClient.PutAsJsonAsync(ApiRoutes.MyLanguage, new UpdateMyLanguageCommand(preferredLanguage), ApiRequests.JsonOptions);

    public static Task<MyAccountResponse?> GetMyAccountAsync(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<MyAccountResponse>(new Uri(ApiRoutes.MyAccount, UriKind.Relative), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PostLegalTermsAcceptanceAsync(this HttpClient httpClient) =>
        httpClient.PostAsync(new Uri(ApiRoutes.MyLegalTermsAcceptance, UriKind.Relative), null);

    public static Task<HttpResponseMessage> PostSwitchAccountAsync(this HttpClient httpClient, string refreshToken, Guid businessId, string kind) =>
        httpClient.PostAsJsonAsync(SwitchRoute, new SwitchBranchCommand(refreshToken, businessId, kind), ApiRequests.JsonOptions);

    public static async Task<OwnerAndStudent> SeedOwnerWhoIsAlsoStudentAsync(this ApiFixture fixture)
    {
        var instructors = await fixture.SeedInstructorScenarioAsync();
        var email = AuthenticationRequests.UniqueEmail();
        using var anonymous = fixture.ApiFactory.CreateClient();
        var signUp = await anonymous.SignUpAsync(fixture.ApiFactory.EmailTransport, AuthenticationRequests.SignUpCommand(email));
        using var ownerClient = fixture.CreateClientWithToken(signUp.AccessToken);
        var ownBusiness = (await ownerClient.ListBranchesAsync()).Single();

        var fees = await instructors.Business.HttpClient.GetMonthlyFeesAsync();
        var clientId = fees!.Clients.Single(client => client.StudentNames.Contains(InstructorScenario.InstructorStudentFullName)).ClientId;
        (await instructors.Business.HttpClient.PostStudentAppInvitationAsync(clientId, email)).EnsureSuccessStatusCode();
        using var accepted = await anonymous.PostAcceptStudentAppInvitationAsync(fixture.ApiFactory.EmailTransport.StudentAppInvitationTokenSentTo(email));
        accepted.EnsureSuccessStatusCode();
        var studentTokens = await accepted.ReadTokensAsync();
        await anonymous.SwitchBranchAsync(studentTokens.RefreshToken, ownBusiness.BusinessId, AccountKinds.Team);

        var ownerTokens = await anonymous.SignInAsync(email);
        var student = new StudentAppScenario(instructors, clientId, email, ownerTokens, fixture.CreateClientWithToken(ownerTokens.AccessToken));
        return new OwnerAndStudent(student, ownBusiness.BusinessId, ownBusiness.Name, email, ownerTokens);
    }
}
