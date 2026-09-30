namespace ClassManager.Api.I.Tests.Endpoints.Coaches.When_coach_publishes_an_announcement;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedCoachScenarioAsync();

        using var response = await scenario.Coach.PostAnnouncementAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
