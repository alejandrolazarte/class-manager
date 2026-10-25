using ClassManager.Security.Passkeys;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Passkeys.When_a_sign_in_ceremony_expired;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_refused(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_refused_Run()
    {
        using var anonymous = fixture.ApiFactory.CreateClient();
        var signUp = await anonymous.SignUpAsync(fixture.ApiFactory.EmailTransport, AuthenticationRequests.SignUpCommand());
        using var owner = fixture.CreateClientWithToken(signUp.AccessToken);
        using var authenticator = new SoftwareAuthenticator();
        await owner.AddPasskeyAsync(authenticator);
        var ceremony = await anonymous.BeginPasskeySignInAsync();
        await using (var securityContext = fixture.CreateSecurityDbContext())
        {
            await securityContext.PasskeyCeremonies
                .Where(storedCeremony => storedCeremony.Kind == PasskeyCeremonyKind.SignIn && storedCeremony.ExpiresAt > BusinessApiFactory.Now)
                .ExecuteUpdateAsync(setters => setters.SetProperty(storedCeremony => storedCeremony.ExpiresAt, BusinessApiFactory.Now));
        }

        using var response = await anonymous.PostPasskeySignInAsync(ceremony.CeremonyId, authenticator.Sign(ceremony.Options));

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
