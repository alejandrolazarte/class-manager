using ClassManager.Core.Abstractions.Security;
using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.Members;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_new_user_accepts_without_accepting_the_legal_terms;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var email = MemberRequests.UniqueInviteeEmail();
        await business.HttpClient.InviteAsync(email, BusinessRole.Viewer);
        using var anonymousClient = fixture.ApiFactory.CreateClient();

        using var response = await anonymousClient.PostAsJsonAsync(
            ApiRoutes.Authentication + ApiRoutes.AcceptInvitation,
            new AcceptInvitationCommand(
                fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email),
                MemberRequests.InviteeFullName,
                MemberRequests.InviteePassword,
                AuthenticationRequests.AdultBirthDate),
            ApiRequests.JsonOptions);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).ShouldContain(AuthenticationErrorCodes.LegalTermsNotAccepted);
    }
}
