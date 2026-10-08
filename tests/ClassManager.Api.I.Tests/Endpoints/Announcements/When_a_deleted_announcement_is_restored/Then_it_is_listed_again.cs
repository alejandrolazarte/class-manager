namespace ClassManager.Api.I.Tests.Endpoints.Announcements.When_a_deleted_announcement_is_restored;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_listed_again(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_listed_again_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var announcement = await business.HttpClient.PublishAnnouncementAsync();
        (await business.HttpClient.DeleteAnnouncementAsync(announcement.Id)).EnsureSuccessStatusCode();

        using var response = await business.HttpClient.RestoreAnnouncementAsync(announcement.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await business.HttpClient.ListAnnouncementsAsync())!.Single().Id.ShouldBe(announcement.Id);
    }
}
