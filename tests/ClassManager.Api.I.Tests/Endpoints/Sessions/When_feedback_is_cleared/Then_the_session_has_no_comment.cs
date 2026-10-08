namespace ClassManager.Api.I.Tests.Endpoints.Sessions.When_feedback_is_cleared;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_session_has_no_comment(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_session_has_no_comment_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();
        var owner = scenario.Business.HttpClient;
        (await owner.PutFeedbackAsync(scenario.InstructorClassGroup.Id, InstructorScenario.ClassDate, scenario.InstructorStudentId, "Muy bien"))
            .EnsureSuccessStatusCode();

        using var cleared = await owner.PutFeedbackAsync(scenario.InstructorClassGroup.Id, InstructorScenario.ClassDate, scenario.InstructorStudentId, " ");

        cleared.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var session = await owner.GetSessionAsync(scenario.InstructorClassGroup.Id, InstructorScenario.ClassDate);
        session!.Students.Single().Feedback.ShouldBeNull();
    }
}
