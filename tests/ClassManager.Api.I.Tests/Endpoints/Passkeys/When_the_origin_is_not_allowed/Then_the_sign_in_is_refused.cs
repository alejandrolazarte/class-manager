namespace ClassManager.Api.I.Tests.Endpoints.Passkeys.When_the_origin_is_not_allowed;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_sign_in_is_refused(ApiFixture fixture)
{
    private const string PhishingOrigin = "https://laztty-login.example.com";

    [Fact]
    public async Task Then_the_sign_in_is_refused_Run()
    {
        using var anonymous = fixture.ApiFactory.CreateClient();
        var signUp = await anonymous.SignUpAsync(fixture.ApiFactory.EmailTransport, AuthenticationRequests.SignUpCommand());
        using var owner = fixture.CreateClientWithToken(signUp.AccessToken);
        using var authenticator = new SoftwareAuthenticator();
        await owner.AddPasskeyAsync(authenticator);
        authenticator.Origin = PhishingOrigin;

        using var response = await anonymous.SignInWithPasskeyAsync(authenticator);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
