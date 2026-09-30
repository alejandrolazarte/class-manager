namespace ClassManager.Api.I.Tests.Endpoints.TeamNotifications.When_coach_turned_on_notifications;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_coach_gets_a_push(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_coach_gets_a_push_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var endpoint = await scenario.Coaches.Coach.SubscribeToTeamPushAsync();

        (await scenario.Family.PutAbsenceAsync(
            scenario.Coaches.CoachStudentId, scenario.Coaches.CoachClassGroup.Id, CoachScenario.ClassDate.AddDays(7))).EnsureSuccessStatusCode();

        var push = await fixture.ApiFactory.PushSender.WaitForPushToAsync(endpoint);
        push.Title.ShouldBe("Tomás avisó que no viene");
    }
}
