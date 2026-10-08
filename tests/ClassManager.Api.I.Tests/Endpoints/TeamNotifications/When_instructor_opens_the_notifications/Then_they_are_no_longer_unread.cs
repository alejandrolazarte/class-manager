namespace ClassManager.Api.I.Tests.Endpoints.TeamNotifications.When_instructor_opens_the_notifications;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_are_no_longer_unread(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_are_no_longer_unread_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        (await scenario.Student.PutAbsenceAsync(
            scenario.Instructors.InstructorStudentId, scenario.Instructors.InstructorClassGroup.Id, InstructorScenario.ClassDate.AddDays(7))).EnsureSuccessStatusCode();

        using var response = await scenario.Instructors.Instructor.PutTeamNotificationsSeenAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var notifications = await scenario.Instructors.Instructor.GetTeamNotificationsAsync();
        notifications!.UnreadCount.ShouldBe(0);
        notifications.Items.Single().IsUnread.ShouldBeFalse();
    }
}
