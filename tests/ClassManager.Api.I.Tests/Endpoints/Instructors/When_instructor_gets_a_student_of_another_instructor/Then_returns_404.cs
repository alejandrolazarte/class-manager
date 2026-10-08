namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_instructor_gets_a_student_of_another_instructor;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        using var response = await scenario.Instructor.GetAsync(new Uri($"{ApiRoutes.Students}/{scenario.OtherStudentId}", UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
