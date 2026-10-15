using ClassManager.Core.UseCases.Announcements;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPush.When_school_reschedules_an_announcement;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_push_waits_for_the_new_time(ApiFixture fixture)
{
    [Fact]
    public async Task Then_its_push_waits_for_the_new_time_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var endpoint = await scenario.SubscribeAsync();
        var httpClient = scenario.Instructors.Business.HttpClient;
        var announcement = await httpClient.PublishAnnouncementAsync(
            new CreateAnnouncementCommand("Feriado del 8 de diciembre", null, PublishOn: BusinessApiFactory.Now.AddDays(2)));
        var newPublishOn = BusinessApiFactory.Now.AddDays(3);

        var rescheduled = await httpClient.UpdateAnnouncementAsync(
            announcement.Id, new UpdateAnnouncementRequest("Feriado del 8 de diciembre", null, PublishOn: newPublishOn));

        rescheduled.PublishedAt.ShouldBe(newPublishOn);
        await using var context = fixture.CreateDbContext(Guid.Empty);
        var task = await context.BackgroundTasks.AsNoTracking().SingleAsync(candidate => candidate.Payload.Contains(announcement.Id.ToString()));
        task.NextAttemptOn.ShouldBe(newPublishOn);
        fixture.ApiFactory.PushSender.SentTo(endpoint).ShouldBeEmpty();
    }
}
