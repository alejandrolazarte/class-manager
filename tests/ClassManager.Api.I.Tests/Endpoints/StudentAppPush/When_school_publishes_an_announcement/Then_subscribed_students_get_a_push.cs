namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPush.When_school_publishes_an_announcement;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_subscribed_students_get_a_push(ApiFixture fixture)
{
    [Fact]
    public async Task Then_subscribed_students_get_a_push_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var endpoint = await scenario.SubscribeAsync();

        (await scenario.Coaches.Business.HttpClient.PostAnnouncementAsync("Lunes cerrado", "Feriado")).EnsureSuccessStatusCode();

        var push = await fixture.ApiFactory.PushSender.WaitForPushToAsync(endpoint);
        push.ShouldBe(new PushMessage("Lunes cerrado", "Feriado", "/student-app/news"));
    }
}
