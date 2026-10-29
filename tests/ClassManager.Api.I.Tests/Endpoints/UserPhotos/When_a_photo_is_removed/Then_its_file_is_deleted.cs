namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_a_photo_is_removed;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_file_is_deleted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_its_file_is_deleted_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var photo = await business.HttpClient.ChangeMyPhotoAsync();

        using var removal = await business.HttpClient.DeleteMyPhotoAsync();

        removal.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await business.HttpClient.GetMyPhotoAsync())!.Url.ShouldBeNull();
        using var download = await fixture.AnonymousClient.GetAsync(new Uri(photo.Url!));
        download.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
