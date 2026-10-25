using ClassManager.Api.I.Tests.Audit;
using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.I.Tests.Endpoints.Passkeys.When_a_passkey_sign_in_fails;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_a_security_event_is_written(ApiFixture fixture)
{
    [Fact]
    public async Task Then_a_security_event_is_written_Run()
    {
        using var anonymous = fixture.ApiFactory.CreateClient();
        using var unknownAuthenticator = new SoftwareAuthenticator();
        var ceremony = await anonymous.BeginPasskeySignInAsync();

        using var response = await anonymous.PostPasskeySignInAsync(ceremony.CeremonyId, unknownAuthenticator.Sign(ceremony.Options));

        var lines = await SecurityEventBlobs.WaitForAsync(fixture, line =>
            SecurityEventBlobs.Text(line, "event") == nameof(SecurityEventType.SignInFailed)
            && SecurityEventBlobs.Text(line, "route") == PasskeyRequests.SignInRoute);
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        lines.ShouldNotBeEmpty();
    }
}
