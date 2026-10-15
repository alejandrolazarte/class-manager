using ClassManager.Core.UseCases.Announcements;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPush.When_a_scheduled_announcement_is_due;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_subscribed_students_get_its_push(ApiFixture fixture)
{
    [Fact]
    public async Task Then_subscribed_students_get_its_push_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var endpoint = await scenario.SubscribeAsync();
        var businessId = scenario.Instructors.Business.Business.Id;
        var announcement = await scenario.Instructors.Business.HttpClient.PublishAnnouncementAsync(
            new CreateAnnouncementCommand("Feriado del 8 de diciembre", "Ese día no hay clases.", PublishOn: BusinessApiFactory.Now.AddDays(2)));
        var due = BusinessApiFactory.Now.AddMinutes(-1);
        await using (var context = fixture.CreateDbContext(businessId))
        {
            await context.Announcements.Where(row => row.Id == announcement.Id)
                .ExecuteUpdateAsync(setters => setters.SetProperty(row => row.PublishedAt, due));
            await context.BackgroundTasks.Where(task => task.Payload.Contains(announcement.Id.ToString()))
                .ExecuteUpdateAsync(setters => setters.SetProperty(task => task.NextAttemptOn, due));
        }

        await fixture.EnqueueEmailAsync(businessId, BackgroundTaskRequests.UniqueRecipient());

        var push = await fixture.ApiFactory.PushSender.WaitForPushToAsync(endpoint);
        push.Title.ShouldBe("Feriado del 8 de diciembre");
    }
}
