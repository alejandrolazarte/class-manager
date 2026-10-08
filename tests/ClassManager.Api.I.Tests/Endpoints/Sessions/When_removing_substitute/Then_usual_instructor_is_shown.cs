namespace ClassManager.Api.I.Tests.Endpoints.Sessions.When_removing_substitute;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_usual_instructor_is_shown(ApiFixture fixture)
{
    [Fact]
    public async Task Then_usual_instructor_is_shown_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();
        var owner = scenario.Business.HttpClient;
        using var assignment = await owner.PutSubstituteAsync(scenario.InstructorClassGroup.Id, InstructorScenario.ClassDate, scenario.OtherInstructorId);

        using var response = await owner.DeleteAsync(
            new Uri($"{SessionRequests.SessionPath(scenario.InstructorClassGroup.Id, InstructorScenario.ClassDate)}{ApiRoutes.Substitute}", UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var session = await owner.GetSessionAsync(scenario.InstructorClassGroup.Id, InstructorScenario.ClassDate);
        session!.InstructorFullName.ShouldBe(InstructorScenario.InstructorFullName);
        session.OriginalInstructorFullName.ShouldBeNull();
    }
}
