using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPush.When_the_push_service_says_the_subscription_expired;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_removed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_removed_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var endpoint = await scenario.SubscribeAsync();
        fixture.ApiFactory.PushSender.Expire(endpoint);

        (await scenario.Instructors.Business.HttpClient.PostAnnouncementAsync("Lunes cerrado", "Feriado")).EnsureSuccessStatusCode();
        await fixture.ApiFactory.PushSender.WaitForPushToAsync(endpoint);

        await using var context = fixture.CreateDbContext(scenario.Instructors.Business.Business.Id);
        var isRemoved = false;
        for (var attempt = 0; attempt < 100 && !isRemoved; attempt++)
        {
            isRemoved = !await context.PushSubscriptions.AnyAsync(saved => saved.Endpoint == endpoint);
            await Task.Delay(TimeSpan.FromMilliseconds(50));
        }

        isRemoved.ShouldBeTrue();
    }
}
