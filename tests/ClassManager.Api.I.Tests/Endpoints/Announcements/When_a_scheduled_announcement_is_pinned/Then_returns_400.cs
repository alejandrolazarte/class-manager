using ClassManager.Core.UseCases.Announcements;

namespace ClassManager.Api.I.Tests.Endpoints.Announcements.When_a_scheduled_announcement_is_pinned;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var announcement = await business.HttpClient.PublishAnnouncementAsync(new CreateAnnouncementCommand(
            "Feriado del 8 de diciembre", null, PublishOn: BusinessApiFactory.Now.AddDays(2)));

        using var response = await business.HttpClient.PinAnnouncementAsync(announcement.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).ShouldContain(AnnouncementErrorCodes.ScheduledCannotBePinned);
    }
}
