namespace ClassManager.Api.I.Tests.Endpoints.Passkeys.When_a_passkey_is_added_with_the_right_password;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_signs_in(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_signs_in_Run()
    {
        using var anonymous = fixture.ApiFactory.CreateClient();
        var signUp = await anonymous.SignUpAsync(fixture.ApiFactory.EmailTransport, AuthenticationRequests.SignUpCommand());
        using var owner = fixture.CreateClientWithToken(signUp.AccessToken);
        using var authenticator = new SoftwareAuthenticator();
        await owner.AddPasskeyAsync(authenticator);

        using var response = await anonymous.SignInWithPasskeyAsync(authenticator);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
