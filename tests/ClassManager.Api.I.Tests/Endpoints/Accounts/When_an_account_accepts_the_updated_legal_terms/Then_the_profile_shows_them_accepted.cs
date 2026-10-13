using ClassManager.Core.Domain.Accounts;
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
        var acceptanceId = (await fixture.LegalTermsAcceptancesOfAsync(command.Email!)).Single().Id;
        await using (var context = fixture.CreateDbContext(Guid.Empty))
        {
            await context.LegalTermsAcceptances
                .Where(acceptance => acceptance.Id == acceptanceId)
                .ExecuteUpdateAsync(acceptance => acceptance.SetProperty(row => row.Version, OlderVersion));
        }

        (await owner.GetMyAccountAsync())!.HasAcceptedCurrentLegalTerms.ShouldBeFalse();

        using (var response = await owner.PostLegalTermsAcceptanceAsync())
        {
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        (await owner.GetMyAccountAsync())!.HasAcceptedCurrentLegalTerms.ShouldBeTrue();
        (await fixture.LegalTermsAcceptancesOfAsync(command.Email!)).Select(acceptance => acceptance.Version)
            .ShouldBe([OlderVersion, LegalTerms.CurrentVersion], ignoreOrder: true);
    }
}
