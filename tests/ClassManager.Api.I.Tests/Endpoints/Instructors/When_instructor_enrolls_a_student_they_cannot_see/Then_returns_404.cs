namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_instructor_enrolls_a_student_they_cannot_see;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        using var response = await scenario.Instructor.PostEnrollmentAsync(scenario.InstructorClassGroup.Id, scenario.OtherStudentId);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
