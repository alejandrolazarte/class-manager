using ClassManager.Core.Domain.Accounts;
using Microsoft.EntityFrameworkCore;

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

        await using var securityContext = fixture.CreateSecurityDbContext();
        var user = await securityContext.Users.SingleAsync(user => user.Email == command.Email);
        user.AcceptedLegalTermsVersion.ShouldBe(LegalTerms.CurrentVersion);
        user.AcceptedLegalTermsOn.ShouldBe(BusinessApiFactory.Now);
    }
}
