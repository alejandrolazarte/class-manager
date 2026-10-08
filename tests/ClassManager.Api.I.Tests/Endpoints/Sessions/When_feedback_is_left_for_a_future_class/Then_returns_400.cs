namespace ClassManager.Api.I.Tests.Endpoints.Sessions.When_feedback_is_left_for_a_future_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        using var response = await scenario.Business.HttpClient.PutFeedbackAsync(
            scenario.InstructorClassGroup.Id, InstructorScenario.ClassDate.AddDays(7), scenario.InstructorStudentId, "Todavía no pasó");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
