using ClassManager.Core.Domain.Images;

namespace ClassManager.Api.I.Tests.Endpoints.UserPhotos.When_a_photo_is_not_an_image;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_refused(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_refused_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PutMyPhotoAsync("not an image"u8.ToArray());

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).ShouldContain(ImageErrorCodes.Unsupported);
    }
}
