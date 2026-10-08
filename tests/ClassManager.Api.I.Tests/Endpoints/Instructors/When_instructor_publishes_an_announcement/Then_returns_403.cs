namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_instructor_publishes_an_announcement;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        using var response = await scenario.Instructor.PostAnnouncementAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
