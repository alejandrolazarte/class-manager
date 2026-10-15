using ClassManager.Core.Emails;

namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_signing_up_with_taken_email;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_email_offers_to_sign_in(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_email_offers_to_sign_in_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var email = AuthenticationRequests.UniqueEmail();
        await client.SignUpAsync(fixture.ApiFactory.EmailTransport, AuthenticationRequests.SignUpCommand(email));

        using var response = await client.PostSignUpAsync(AuthenticationRequests.SignUpCommand(email.ToUpperInvariant()));

        response.StatusCode.ShouldBe(HttpStatusCode.Accepted);
        fixture.ApiFactory.EmailTransport.SentTo(email)[^1].Subject.ShouldBe(EmailTexts.Spanish.SignUpAccountExists(string.Empty).Subject);
    }
}
