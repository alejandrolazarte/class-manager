namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_instructor_assigns_a_substitute;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        using var response = await scenario.Instructor.PutSubstituteAsync(
            scenario.InstructorClassGroup.Id, InstructorScenario.ClassDate, scenario.OtherInstructorId);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
