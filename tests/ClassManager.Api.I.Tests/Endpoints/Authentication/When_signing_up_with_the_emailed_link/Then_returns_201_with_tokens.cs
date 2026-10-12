namespace ClassManager.Api.I.Tests.Endpoints.Authentication.When_signing_up_with_the_emailed_link;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_201_with_tokens(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_201_with_tokens_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();
        var token = await client.RequestSignUpLinkAsync(fixture.ApiFactory.EmailTransport, AuthenticationRequests.SignUpCommand());

        using var response = await client.PostCompleteSignUpAsync(token);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var tokens = await response.ReadTokensAsync();
        tokens.AccessToken.ShouldNotBeNullOrWhiteSpace();
        tokens.RefreshToken.ShouldNotBeNullOrWhiteSpace();
        tokens.AccessTokenExpiresAt.ShouldBe(BusinessApiFactory.Now.AddMinutes(15));
        tokens.RefreshTokenExpiresAt.ShouldBe(BusinessApiFactory.Now.AddDays(30));
    }
}
