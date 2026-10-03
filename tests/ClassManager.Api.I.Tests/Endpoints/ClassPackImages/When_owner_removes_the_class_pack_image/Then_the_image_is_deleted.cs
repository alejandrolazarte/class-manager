using ClassManager.Core.UseCases.ClassPacks;

namespace ClassManager.Api.I.Tests.Endpoints.ClassPackImages.When_owner_removes_the_class_pack_image;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_image_is_deleted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_image_is_deleted_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classPack = await business.HttpClient.CreateClassPackAsync();
        var imageUrl = (await business.HttpClient.SetClassPackImageAsync(classPack.Id)).ImageUrl!;

        using var response = await business.HttpClient.DeleteClassPackImageAsync(classPack.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<ClassPackResponse>(ApiRequests.JsonOptions))!.ImageUrl.ShouldBeNull();
        using var removedImage = await fixture.AnonymousClient.GetAsync(new Uri(imageUrl));
        removedImage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
