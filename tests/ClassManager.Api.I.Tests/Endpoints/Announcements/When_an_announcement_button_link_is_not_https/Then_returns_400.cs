using ClassManager.Core.Domain.Announcements;
using ClassManager.Core.UseCases.Announcements;

namespace ClassManager.Api.I.Tests.Endpoints.Announcements.When_an_announcement_button_link_is_not_https;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PostAnnouncementAsync(new CreateAnnouncementCommand(
            "Inscripciones abiertas", null, Action: AnnouncementAction.Link, ActionUrl: "http://example.com"));

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
