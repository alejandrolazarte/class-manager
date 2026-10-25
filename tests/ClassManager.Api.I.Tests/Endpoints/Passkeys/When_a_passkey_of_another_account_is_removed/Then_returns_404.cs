namespace ClassManager.Api.I.Tests.Endpoints.Passkeys.When_a_passkey_of_another_account_is_removed;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        using var anonymous = fixture.ApiFactory.CreateClient();
        var ownerSignUp = await anonymous.SignUpAsync(fixture.ApiFactory.EmailTransport, AuthenticationRequests.SignUpCommand());
        var otherSignUp = await anonymous.SignUpAsync(fixture.ApiFactory.EmailTransport, AuthenticationRequests.SignUpCommand());
        using var owner = fixture.CreateClientWithToken(ownerSignUp.AccessToken);
        using var other = fixture.CreateClientWithToken(otherSignUp.AccessToken);
        using var authenticator = new SoftwareAuthenticator();
        var passkey = await owner.AddPasskeyAsync(authenticator);

        using var response = await other.DeletePasskeyAsync(passkey.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        (await owner.ListPasskeysAsync()).ShouldHaveSingleItem();
    }
}
