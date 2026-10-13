using ClassManager.Core.Domain.Accounts;

namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_signing_up_with_the_emailed_link;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_account_records_the_accepted_legal_terms(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_account_records_the_accepted_legal_terms_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var command = AuthenticationRequests.SignUpCommand();

        await client.SignUpAsync(fixture.ApiFactory.EmailTransport, command);

        var acceptance = (await fixture.LegalTermsAcceptancesOfAsync(command.Email!)).ShouldHaveSingleItem();
        acceptance.Version.ShouldBe(LegalTerms.CurrentVersion);
        acceptance.AcceptedOn.ShouldBe(BusinessApiFactory.Now);
    }
}
