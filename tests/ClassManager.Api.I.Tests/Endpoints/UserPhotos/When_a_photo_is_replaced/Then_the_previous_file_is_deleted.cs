namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_a_photo_is_replaced;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_previous_file_is_deleted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_previous_file_is_deleted_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var previousPhoto = await business.HttpClient.ChangeMyPhotoAsync();

        var newPhoto = await business.HttpClient.ChangeMyPhotoAsync();

        using var previousDownload = await fixture.AnonymousClient.GetAsync(new Uri(previousPhoto.Url!));
        previousDownload.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        newPhoto.Url.ShouldNotBe(previousPhoto.Url);
    }
}
