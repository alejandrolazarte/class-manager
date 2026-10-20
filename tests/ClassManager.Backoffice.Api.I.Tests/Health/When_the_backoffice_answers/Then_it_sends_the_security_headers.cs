namespace ClassManager.Backoffice.Api.I.Tests.Health.When_the_backoffice_answers;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_sends_the_security_headers(BackofficeApiFixture fixture)
{
    private const string HealthRoute = "/health";

    [Fact]
    public async Task Then_it_sends_the_security_headers_Run()
    {
        using var client = fixture.ApiFactory.CreateHttpsClient();

        using var response = await client.GetAsync(new Uri(HealthRoute, UriKind.Relative));

        (response.Headers.GetValues("X-Content-Type-Options").Single(),
            response.Headers.GetValues("X-Frame-Options").Single(),
            response.Headers.GetValues("Referrer-Policy").Single())
            .ShouldBe(("nosniff", "DENY", "no-referrer"));
    }
}
