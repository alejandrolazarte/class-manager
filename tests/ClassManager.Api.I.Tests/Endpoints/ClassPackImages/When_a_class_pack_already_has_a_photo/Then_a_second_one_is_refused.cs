using ClassManager.Core.Domain.Images;

namespace ClassManager.Api.I.Tests.Endpoints.ClassPackImages.When_a_class_pack_already_has_a_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_a_second_one_is_refused(ApiFixture fixture)
{
    [Fact]
    public async Task Then_a_second_one_is_refused_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classPack = await business.HttpClient.CreateClassPackAsync();
        await business.HttpClient.AddClassPackImageAsync(classPack.Id);

        using var response = await business.HttpClient.PostClassPackImageAsync(classPack.Id, CatalogImageRequests.PngImage);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await response.Content.ReadAsStringAsync()).ShouldContain(ImageErrorCodes.LimitReached);
    }
}
