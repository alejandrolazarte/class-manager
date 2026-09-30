namespace ClassManager.Api.I.Tests.Endpoints.Sessions.When_feedback_is_left_for_a_future_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var scenario = await fixture.SeedCoachScenarioAsync();

        using var response = await scenario.Business.HttpClient.PutFeedbackAsync(
            scenario.CoachClassGroup.Id, CoachScenario.ClassDate.AddDays(7), scenario.CoachStudentId, "Todavía no pasó");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
