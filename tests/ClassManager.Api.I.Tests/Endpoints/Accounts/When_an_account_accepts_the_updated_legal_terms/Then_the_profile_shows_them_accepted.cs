using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Accounts.When_an_account_accepts_the_updated_legal_terms;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_profile_shows_them_accepted(ApiFixture fixture)
{
    private const string OlderVersion = "2020-01-01";

    [Fact]
    public async Task Then_the_profile_shows_them_accepted_Run()
    {
        using var anonymous = fixture.ApiFactory.CreateClient();
        var command = AuthenticationRequests.SignUpCommand();
        var tokens = await anonymous.SignUpAsync(fixture.ApiFactory.EmailTransport, command);
        using var owner = fixture.CreateClientWithToken(tokens.AccessToken);
        await using (var securityContext = fixture.CreateSecurityDbContext())
        {
            await securityContext.Users
                .Where(user => user.Email == command.Email)
                .ExecuteUpdateAsync(user => user.SetProperty(account => account.AcceptedLegalTermsVersion, OlderVersion));
        }

        (await owner.GetMyAccountAsync())!.HasAcceptedCurrentLegalTerms.ShouldBeFalse();

        using (var response = await owner.PostLegalTermsAcceptanceAsync())
        {
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        (await owner.GetMyAccountAsync())!.HasAcceptedCurrentLegalTerms.ShouldBeTrue();
    }
}
