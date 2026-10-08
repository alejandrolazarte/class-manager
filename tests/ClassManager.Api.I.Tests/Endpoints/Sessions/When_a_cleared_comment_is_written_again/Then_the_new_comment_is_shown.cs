namespace ClassManager.Api.I.Tests.Endpoints.Sessions.When_a_cleared_comment_is_written_again;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_new_comment_is_shown(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_new_comment_is_shown_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();
        var owner = scenario.Business.HttpClient;
        (await owner.PutFeedbackAsync(scenario.InstructorClassGroup.Id, InstructorScenario.ClassDate, scenario.InstructorStudentId, "Muy bien"))
            .EnsureSuccessStatusCode();
        (await owner.PutFeedbackAsync(scenario.InstructorClassGroup.Id, InstructorScenario.ClassDate, scenario.InstructorStudentId, " "))
            .EnsureSuccessStatusCode();

        using var response = await owner.PutFeedbackAsync(scenario.InstructorClassGroup.Id, InstructorScenario.ClassDate, scenario.InstructorStudentId, "Mejoró la patada");

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var session = await owner.GetSessionAsync(scenario.InstructorClassGroup.Id, InstructorScenario.ClassDate);
        session!.Students.Single().Feedback.ShouldBe("Mejoró la patada");
    }
}
