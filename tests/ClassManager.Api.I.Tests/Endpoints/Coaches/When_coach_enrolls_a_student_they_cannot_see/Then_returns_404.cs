namespace ClassManager.Api.I.Tests.Endpoints.Coaches.When_coach_enrolls_a_student_they_cannot_see;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var scenario = await fixture.SeedCoachScenarioAsync();

        using var response = await scenario.Coach.PostEnrollmentAsync(scenario.CoachClassGroup.Id, scenario.OtherStudentId);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
