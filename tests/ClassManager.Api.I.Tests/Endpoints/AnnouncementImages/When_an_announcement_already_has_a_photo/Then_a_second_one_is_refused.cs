using ClassManager.Core.Domain.Images;

namespace ClassManager.Api.I.Tests.Endpoints.AnnouncementImages.When_an_announcement_already_has_a_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_a_second_one_is_refused(ApiFixture fixture)
{
    [Fact]
    public async Task Then_a_second_one_is_refused_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var announcement = await business.HttpClient.PublishAnnouncementAsync();
        await business.HttpClient.AddAnnouncementImageAsync(announcement.Id);

        using var response = await business.HttpClient.PostAnnouncementImageAsync(announcement.Id, CatalogImageRequests.PngImage);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await response.Content.ReadAsStringAsync()).ShouldContain(ImageErrorCodes.LimitReached);
    }
}
