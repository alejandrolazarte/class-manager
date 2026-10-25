namespace ClassManager.Api.I.Tests.Endpoints.Passkeys.When_signing_in_with_a_passkey_from_the_web;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_refresh_token_is_a_cookie(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_refresh_token_is_a_cookie_Run()
    {
        using var anonymous = fixture.ApiFactory.CreateClient();
        var signUp = await anonymous.SignUpAsync(fixture.ApiFactory.EmailTransport, AuthenticationRequests.SignUpCommand());
        using var owner = fixture.CreateClientWithToken(signUp.AccessToken);
        using var authenticator = new SoftwareAuthenticator();
        await owner.AddPasskeyAsync(authenticator);
        var ceremony = await anonymous.BeginPasskeySignInAsync();

        using var response = await anonymous.PostFromWebAsync(
            PasskeyRequests.SignInRoute,
            PasskeyRequests.SignInCommand(ceremony.CeremonyId, authenticator.Sign(ceremony.Options)));

        response.RefreshTokenCookieOf().HttpOnly.ShouldBeTrue();
    }
}
