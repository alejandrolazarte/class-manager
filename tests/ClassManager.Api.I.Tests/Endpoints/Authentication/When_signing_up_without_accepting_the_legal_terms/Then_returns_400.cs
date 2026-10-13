using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_signing_up_without_accepting_the_legal_terms;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var command = AuthenticationRequests.SignUpCommand() with { AcceptsLegalTerms = false };

        using var response = await client.PostSignUpAsync(command);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).ShouldContain(AuthenticationErrorCodes.LegalTermsNotAccepted);
        fixture.ApiFactory.EmailTransport.SentTo(command.Email!).ShouldBeEmpty();
    }
}
