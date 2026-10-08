namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_instructor_comments_on_another_instructor_session;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        using var response = await scenario.Instructor.PutFeedbackAsync(
            scenario.OtherClassGroup.Id, InstructorScenario.ClassDate, scenario.OtherStudentId, "No es mi clase");

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
