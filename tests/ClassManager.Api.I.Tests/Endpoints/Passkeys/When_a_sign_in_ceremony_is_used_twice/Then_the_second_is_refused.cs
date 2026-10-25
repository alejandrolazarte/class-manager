namespace ClassManager.Api.I.Tests.Endpoints.Passkeys.When_a_sign_in_ceremony_is_used_twice;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_second_is_refused(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_second_is_refused_Run()
    {
        using var anonymous = fixture.ApiFactory.CreateClient();
        var signUp = await anonymous.SignUpAsync(fixture.ApiFactory.EmailTransport, AuthenticationRequests.SignUpCommand());
        using var owner = fixture.CreateClientWithToken(signUp.AccessToken);
        using var authenticator = new SoftwareAuthenticator();
        await owner.AddPasskeyAsync(authenticator);
        var ceremony = await anonymous.BeginPasskeySignInAsync();
        var credential = authenticator.Sign(ceremony.Options);
        using var first = await anonymous.PostPasskeySignInAsync(ceremony.CeremonyId, credential);

        using var second = await anonymous.PostPasskeySignInAsync(ceremony.CeremonyId, credential);

        first.StatusCode.ShouldBe(HttpStatusCode.OK);
        second.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
