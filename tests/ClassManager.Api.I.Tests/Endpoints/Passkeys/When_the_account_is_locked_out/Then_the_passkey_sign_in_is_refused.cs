namespace ClassManager.Api.I.Tests.Endpoints.Passkeys.When_the_account_is_locked_out;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_passkey_sign_in_is_refused(ApiFixture fixture)
{
    private const int FailedAttemptsToLockOut = 5;

    [Fact]
    public async Task Then_the_passkey_sign_in_is_refused_Run()
    {
        using var anonymous = fixture.ApiFactory.CreateClient();
        var command = AuthenticationRequests.SignUpCommand();
        var signUp = await anonymous.SignUpAsync(fixture.ApiFactory.EmailTransport, command);
        using var owner = fixture.CreateClientWithToken(signUp.AccessToken);
        using var authenticator = new SoftwareAuthenticator();
        await owner.AddPasskeyAsync(authenticator);
        for (var attempt = 0; attempt < FailedAttemptsToLockOut; attempt++)
        {
            using var failed = await anonymous.PostSignInAsync(command.Email!, AuthenticationRequests.WrongPassword);
        }

        using var response = await anonymous.SignInWithPasskeyAsync(authenticator);

        response.StatusCode.ShouldBe(HttpStatusCode.Locked);
    }
}
