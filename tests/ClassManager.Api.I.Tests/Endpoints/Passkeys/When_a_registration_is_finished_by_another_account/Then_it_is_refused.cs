using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.Passkeys.When_a_registration_is_finished_by_another_account;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_refused(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_refused_Run()
    {
        using var anonymous = fixture.ApiFactory.CreateClient();
        var victimSignUp = await anonymous.SignUpAsync(fixture.ApiFactory.EmailTransport, AuthenticationRequests.SignUpCommand());
        var attackerSignUp = await anonymous.SignUpAsync(fixture.ApiFactory.EmailTransport, AuthenticationRequests.SignUpCommand());
        using var victim = fixture.CreateClientWithToken(victimSignUp.AccessToken);
        using var attacker = fixture.CreateClientWithToken(attackerSignUp.AccessToken);
        using var authenticator = new SoftwareAuthenticator();
        var ceremony = await attacker.BeginRegistrationAsync();

        using var response = await victim.PostPasskeyAsync(ceremony.CeremonyId, authenticator.CreateCredential(ceremony.Options));

        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(AuthenticationErrorCodes.InvalidPasskeyCeremony);
    }
}
