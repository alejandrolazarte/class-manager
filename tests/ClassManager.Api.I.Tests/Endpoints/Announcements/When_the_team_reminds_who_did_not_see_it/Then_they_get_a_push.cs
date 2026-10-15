namespace ClassManager.Api.I.Tests.Endpoints.Announcements.When_the_team_reminds_who_did_not_see_it;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_get_a_push(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_get_a_push_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var endpoint = await scenario.SubscribeAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var announcement = await owner.PublishAnnouncementAsync();
        await fixture.ApiFactory.PushSender.WaitForPushToAsync(endpoint);

        var reminder = await owner.RemindAnnouncementAsync(announcement.Id);

        reminder.RemindedCount.ShouldBe(1);
        await WaitForSecondPushAsync(endpoint);
    }

    private async Task WaitForSecondPushAsync(string endpoint)
    {
        var deadline = DateTimeOffset.UtcNow.AddSeconds(10);
        while (fixture.ApiFactory.PushSender.SentTo(endpoint).Count < 2)
        {
            DateTimeOffset.UtcNow.ShouldBeLessThan(deadline);
            await Task.Delay(50);
        }
    }
}
