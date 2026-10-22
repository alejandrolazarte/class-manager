using ClassManager.Core.UseCases.Announcements;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPush.When_school_schedules_an_announcement;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_push_waits_for_the_publish_time(ApiFixture fixture)
{
    [Fact]
    public async Task Then_its_push_waits_for_the_publish_time_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var endpoint = await scenario.SubscribeAsync();
        var publishOn = BusinessApiFactory.Now.AddDays(2);

        var announcement = await scenario.Instructors.Business.HttpClient.PublishAnnouncementAsync(
            new CreateAnnouncementCommand("Feriado del 8 de diciembre", null, PublishOn: publishOn));

        await using var context = fixture.CreateDbContext(0L);
        var task = await context.BackgroundTasks.AsNoTracking().SingleAsync(candidate => candidate.Payload.Contains(BackgroundTaskRequests.ScheduledAnnouncementPayloadEnd(announcement.Id)));
        task.NextAttemptOn.ShouldBe(publishOn);
        fixture.ApiFactory.PushSender.SentTo(endpoint).ShouldBeEmpty();
    }
}
