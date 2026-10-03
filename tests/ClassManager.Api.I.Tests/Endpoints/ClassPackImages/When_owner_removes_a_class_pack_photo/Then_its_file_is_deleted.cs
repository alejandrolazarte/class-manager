using ClassManager.Core.UseCases.ClassPacks;

namespace ClassManager.Api.I.Tests.Endpoints.ClassPackImages.When_owner_removes_a_class_pack_photo;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_file_is_deleted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_its_file_is_deleted_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classPack = await business.HttpClient.CreateClassPackAsync();
        var image = (await business.HttpClient.AddClassPackImageAsync(classPack.Id)).Images.Single();

        using var response = await business.HttpClient.DeleteClassPackImageAsync(classPack.Id, image.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<ClassPackResponse>(ApiRequests.JsonOptions))!.Images.ShouldBeEmpty();
        using var removedImage = await fixture.AnonymousClient.GetAsync(new Uri(image.Url));
        removedImage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
