using ClassManager.Core.UseCases.Announcements;

namespace ClassManager.Api.I.Tests.Endpoints.Announcements.When_an_announcement_of_another_business_is_edited;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var published = await otherBusiness.HttpClient.PublishAnnouncementAsync();

        using var response = await business.HttpClient.PutAnnouncementAsync(published.Id, new UpdateAnnouncementRequest("Otro título", null));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
