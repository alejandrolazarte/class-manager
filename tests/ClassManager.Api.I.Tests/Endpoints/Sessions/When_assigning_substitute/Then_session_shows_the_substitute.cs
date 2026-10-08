namespace ClassManager.Api.I.Tests.Endpoints.Sessions.When_assigning_substitute;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_session_shows_the_substitute(ApiFixture fixture)
{
    [Fact]
    public async Task Then_session_shows_the_substitute_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();
        var owner = scenario.Business.HttpClient;

        using var response = await owner.PutSubstituteAsync(scenario.InstructorClassGroup.Id, InstructorScenario.ClassDate, scenario.OtherInstructorId);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var session = await owner.GetSessionAsync(scenario.InstructorClassGroup.Id, InstructorScenario.ClassDate);
        session!.InstructorId.ShouldBe(scenario.OtherInstructorId);
        session.InstructorFullName.ShouldBe(InstructorScenario.OtherInstructorFullName);
        session.OriginalInstructorFullName.ShouldBe(InstructorScenario.InstructorFullName);
    }
}
