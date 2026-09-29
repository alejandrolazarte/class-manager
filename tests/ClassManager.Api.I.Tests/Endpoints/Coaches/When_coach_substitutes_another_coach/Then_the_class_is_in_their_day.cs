namespace ClassManager.Api.I.Tests.Endpoints.Coaches.When_coach_substitutes_another_coach;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_class_is_in_their_day(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_class_is_in_their_day_Run()
    {
        var scenario = await fixture.SeedCoachScenarioAsync();
        using var assignment = await scenario.Business.HttpClient.PutSubstituteAsync(
            scenario.OtherClassGroup.Id, CoachScenario.ClassDate, scenario.CoachInstructorId);

        var sessions = await scenario.Coach.GetDayAsync(CoachScenario.ClassDate);

        sessions!.Select(session => session.ClassGroupName).ShouldBe([CoachScenario.CoachClassGroupName, CoachScenario.OtherClassGroupName]);
        sessions![1].InstructorFullName.ShouldBe(CoachScenario.CoachFullName);
        sessions![1].OriginalInstructorFullName.ShouldBe(CoachScenario.OtherInstructorFullName);
    }
}
