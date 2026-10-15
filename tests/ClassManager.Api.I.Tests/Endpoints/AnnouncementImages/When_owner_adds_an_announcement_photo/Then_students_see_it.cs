using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Endpoints.AnnouncementImages.When_owner_adds_an_announcement_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_students_see_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_students_see_it_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business;
        var announcement = await owner.HttpClient.PublishAnnouncementAsync();

        var image = (await owner.HttpClient.AddAnnouncementImageAsync(announcement.Id)).Images.Single();

        image.Url.ShouldContain($"/{owner.Business.Id}/announcements/{announcement.Id}/");
        var news = await scenario.Student.GetStudentAppNewsAsync();
        news!.Items.Single(item => item.Kind == StudentAppNewsKind.Announcement).ImageUrl.ShouldBe(image.Url);
        using var response = await fixture.AnonymousClient.GetAsync(new Uri(image.Url));
        (await response.Content.ReadAsByteArrayAsync()).ShouldBe(CatalogImageRequests.PngImage);
    }
}
