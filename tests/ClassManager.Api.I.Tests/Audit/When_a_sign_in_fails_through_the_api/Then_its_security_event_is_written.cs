using ClassManager.Core.UseCases.Authentication;
using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.I.Tests.Audit.When_a_sign_in_fails_through_the_api;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_security_event_is_written(ApiFixture fixture)
{
    private const string WrongPassword = "not-the-password-1234";

    [Fact]
    public async Task Then_its_security_event_is_written_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();

        using var response = await client.PostAsJsonAsync(
            AuthenticationRequests.SignInRoute,
            new SignInCommand(AuthenticationRequests.UniqueEmail(), WrongPassword),
            ApiRequests.JsonOptions);

        var lines = await SecurityEventBlobs.WaitForAsync(fixture, line =>
            SecurityEventBlobs.Text(line, "event") == nameof(SecurityEventType.SignInFailed)
            && SecurityEventBlobs.Text(line, "route") == AuthenticationRequests.SignInRoute);
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        lines.ShouldNotContain(line => line.GetRawText().Contains(WrongPassword, StringComparison.Ordinal));
    }
}
