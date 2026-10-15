namespace ClassManager.Api.I.Tests.Endpoints.StudentAppNews.When_the_student_opens_the_news;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_team_sees_it_was_seen(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_team_sees_it_was_seen_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var announcement = await owner.PublishAnnouncementAsync();

        (await scenario.Student.PutStudentAppNewsSeenAsync()).EnsureSuccessStatusCode();

        var listed = (await owner.ListAnnouncementsAsync())!.Single(item => item.Id == announcement.Id);
        listed.SeenCount.ShouldBe(1);
        listed.RecipientCount.ShouldBe(1);
    }
}
