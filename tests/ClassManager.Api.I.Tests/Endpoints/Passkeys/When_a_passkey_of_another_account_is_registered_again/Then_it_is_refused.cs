using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.Passkeys.When_a_passkey_of_another_account_is_registered_again;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_refused(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_refused_Run()
    {
        using var anonymous = fixture.ApiFactory.CreateClient();
        var ownerSignUp = await anonymous.SignUpAsync(fixture.ApiFactory.EmailTransport, AuthenticationRequests.SignUpCommand());
        var otherSignUp = await anonymous.SignUpAsync(fixture.ApiFactory.EmailTransport, AuthenticationRequests.SignUpCommand());
        using var owner = fixture.CreateClientWithToken(ownerSignUp.AccessToken);
        using var other = fixture.CreateClientWithToken(otherSignUp.AccessToken);
        using var authenticator = new SoftwareAuthenticator();
        await owner.AddPasskeyAsync(authenticator);
        var ceremony = await other.BeginRegistrationAsync();

        using var response = await other.PostPasskeyAsync(ceremony.CeremonyId, authenticator.CreateCredential(ceremony.Options));

        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(AuthenticationErrorCodes.InvalidPasskey);
    }
}
