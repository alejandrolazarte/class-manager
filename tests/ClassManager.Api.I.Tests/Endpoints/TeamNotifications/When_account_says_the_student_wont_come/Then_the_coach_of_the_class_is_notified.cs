namespace ClassManager.Api.I.Tests.Endpoints.TeamNotifications.When_account_says_the_student_wont_come;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_coach_of_the_class_is_notified(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_coach_of_the_class_is_notified_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var classGroupId = scenario.Coaches.CoachClassGroup.Id;
        var nextWeek = CoachScenario.ClassDate.AddDays(7);

        (await scenario.Student.PutAbsenceAsync(scenario.Coaches.CoachStudentId, classGroupId, nextWeek)).EnsureSuccessStatusCode();

        var notifications = await scenario.Coaches.Coach.GetTeamNotificationsAsync();
        notifications!.UnreadCount.ShouldBe(1);
        var notification = notifications.Items.Single();
        notification.Title.ShouldBe("Tomás avisó que no viene");
        notification.Url.ShouldBe($"/today/{classGroupId}/{nextWeek:yyyy-MM-dd}");
    }
}
