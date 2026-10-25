using ClassManager.Tenancy;

namespace ClassManager.Api.I.Tests.Endpoints.Passkeys.When_a_team_member_signs_in_with_a_passkey;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_session_starts_in_their_business(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_session_starts_in_their_business_Run()
    {
        using var anonymous = fixture.ApiFactory.CreateClient();
        var signUp = await anonymous.SignUpAsync(fixture.ApiFactory.EmailTransport, AuthenticationRequests.SignUpCommand());
        using var owner = fixture.CreateClientWithToken(signUp.AccessToken);
        using var authenticator = new SoftwareAuthenticator();
        await owner.AddPasskeyAsync(authenticator);

        using var response = await anonymous.SignInWithPasskeyAsync(authenticator);

        var tokens = await response.ReadTokensAsync();
        AuthenticationRequests.AccessTokenClaim(tokens.AccessToken, TenantClaimTypes.TenantId)
            .ShouldBe(AuthenticationRequests.AccessTokenClaim(signUp.AccessToken, TenantClaimTypes.TenantId));
    }
}
