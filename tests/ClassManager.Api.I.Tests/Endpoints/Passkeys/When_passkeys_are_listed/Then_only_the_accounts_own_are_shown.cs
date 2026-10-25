namespace ClassManager.Api.I.Tests.Endpoints.Passkeys.When_passkeys_are_listed;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_the_accounts_own_are_shown(ApiFixture fixture)
{
    [Fact]
    public async Task Then_only_the_accounts_own_are_shown_Run()
    {
        using var anonymous = fixture.ApiFactory.CreateClient();
        var ownerSignUp = await anonymous.SignUpAsync(fixture.ApiFactory.EmailTransport, AuthenticationRequests.SignUpCommand());
        var otherSignUp = await anonymous.SignUpAsync(fixture.ApiFactory.EmailTransport, AuthenticationRequests.SignUpCommand());
        using var owner = fixture.CreateClientWithToken(ownerSignUp.AccessToken);
        using var other = fixture.CreateClientWithToken(otherSignUp.AccessToken);
        using var ownerAuthenticator = new SoftwareAuthenticator();
        using var otherAuthenticator = new SoftwareAuthenticator();
        await owner.AddPasskeyAsync(ownerAuthenticator);
        await other.AddPasskeyAsync(otherAuthenticator);

        var passkeys = await owner.ListPasskeysAsync();

        passkeys.ShouldHaveSingleItem().Id.ShouldBe(ownerAuthenticator.CredentialId);
        passkeys[0].Name.ShouldBe(PasskeyRequests.PasskeyName);
    }
}
