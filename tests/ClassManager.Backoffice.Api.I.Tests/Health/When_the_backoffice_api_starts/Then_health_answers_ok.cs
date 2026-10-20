namespace ClassManager.Backoffice.Api.I.Tests.Health.When_the_backoffice_api_starts;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_health_answers_ok(BackofficeApiFixture fixture)
{
    private const string HealthRoute = "/health";

    [Fact]
    public async Task Then_health_answers_ok_Run()
    {
        using var client = fixture.ApiFactory.CreateClient();

        using var response = await client.GetAsync(HealthRoute);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
