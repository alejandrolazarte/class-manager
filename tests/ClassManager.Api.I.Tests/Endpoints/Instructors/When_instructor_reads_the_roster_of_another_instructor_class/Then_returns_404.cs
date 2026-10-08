namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_instructor_reads_the_roster_of_another_instructor_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        using var response = await scenario.Instructor.GetAsync(
            new Uri($"{ApiRoutes.ClassGroups}/{scenario.OtherClassGroup.Id}{ApiRoutes.EnrollmentsSegment}", UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
