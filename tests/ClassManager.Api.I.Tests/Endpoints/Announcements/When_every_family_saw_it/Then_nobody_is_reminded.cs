namespace ClassManager.Api.I.Tests.Endpoints.Announcements.When_every_family_saw_it;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_nobody_is_reminded(ApiFixture fixture)
{
    [Fact]
    public async Task Then_nobody_is_reminded_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var announcement = await owner.PublishAnnouncementAsync();
        (await scenario.Student.PutStudentAppNewsSeenAsync()).EnsureSuccessStatusCode();

        var reminder = await owner.RemindAnnouncementAsync(announcement.Id);

        reminder.RemindedCount.ShouldBe(0);
    }
}
