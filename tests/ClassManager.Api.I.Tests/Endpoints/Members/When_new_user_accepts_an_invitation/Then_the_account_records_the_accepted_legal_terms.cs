using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_new_user_accepts_an_invitation;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_account_records_the_accepted_legal_terms(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_account_records_the_accepted_legal_terms_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var email = MemberRequests.UniqueInviteeEmail();
        await business.HttpClient.InviteAsync(email, BusinessRole.Viewer);
        using var anonymousClient = fixture.ApiFactory.CreateClient();

        await anonymousClient.AcceptInvitationAsync(fixture.ApiFactory.EmailTransport.InvitationTokenSentTo(email));

        (await fixture.LegalTermsAcceptancesOfAsync(email)).ShouldHaveSingleItem().Version.ShouldBe(LegalTerms.CurrentVersion);
    }
}
