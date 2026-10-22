using ClassManager.Core.UseCases.Announcements;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPush.When_school_publishes_a_scheduled_announcement_now;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_subscribed_students_get_its_push(ApiFixture fixture)
{
    [Fact]
    public async Task Then_subscribed_students_get_its_push_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var endpoint = await scenario.SubscribeAsync();
        var httpClient = scenario.Instructors.Business.HttpClient;
        var announcement = await httpClient.PublishAnnouncementAsync(
            new CreateAnnouncementCommand("Feriado del 8 de diciembre", null, PublishOn: BusinessApiFactory.Now.AddDays(2)));

        var published = await httpClient.UpdateAnnouncementAsync(
            announcement.Id, new UpdateAnnouncementRequest("Feriado del 8 de diciembre", "Ese día no hay clases."));

        var push = await fixture.ApiFactory.PushSender.WaitForPushToAsync(endpoint);
        push.Title.ShouldBe("Feriado del 8 de diciembre");
        published.IsScheduled.ShouldBeFalse();
        await using var context = fixture.CreateDbContext(0L);
        (await context.BackgroundTasks.AsNoTracking().AnyAsync(candidate => candidate.Payload.Contains(BackgroundTaskRequests.ScheduledAnnouncementPayloadEnd(announcement.Id))))
            .ShouldBeFalse();
    }
}
