using ClassManager.Core.UseCases.Announcements;

namespace ClassManager.Api.I.Tests.Endpoints.Announcements.When_a_scheduled_announcement_is_published_pinned;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PostAnnouncementAsync(new CreateAnnouncementCommand(
            "Feriado del 8 de diciembre", null, IsPinned: true, PublishOn: BusinessApiFactory.Now.AddDays(2)));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).ShouldContain(AnnouncementErrorCodes.ScheduledCannotBePinned);
    }
}
