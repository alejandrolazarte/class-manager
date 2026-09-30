namespace ClassManager.Api.I.Tests.Endpoints.Families.When_coach_leaves_a_comment_after_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_family_sees_it_on_home(ApiFixture fixture)
{
    private const string Comment = "Hoy mantuvo la postura en todo el circuito.";

    [Fact]
    public async Task Then_the_family_sees_it_on_home_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        using (var response = await scenario.Coaches.Coach.PutFeedbackAsync(
            scenario.Coaches.CoachClassGroup.Id, CoachScenario.ClassDate, scenario.Coaches.CoachStudentId, Comment))
        {
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        var home = await scenario.Family.GetFamilyHomeAsync();

        var feedback = home!.Students.Single().LatestFeedback;
        feedback.ShouldNotBeNull();
        feedback.Text.ShouldBe(Comment);
        feedback.InstructorFullName.ShouldBe(CoachScenario.CoachFullName);
        feedback.ClassName.ShouldBe(CoachScenario.CoachClassGroupName);
        feedback.Date.ShouldBe(CoachScenario.ClassDate);
    }
}
