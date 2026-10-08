using ClassManager.Core.UseCases.ClassGroups;

namespace ClassManager.Api.I.Tests.Endpoints.ClassMaterial.When_owner_removes_the_class_material_file;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_file_is_deleted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_its_file_is_deleted_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var instructor = await business.HttpClient.CreateInstructorAsync();
        var classGroup = await business.HttpClient.CreateClassGroupAsync(ClassGroupRequests.ClassGroupFor(instructor.Id));
        var file = (await business.HttpClient.UploadClassMaterialFileAsync(classGroup.Id)).MaterialFile!;

        using var response = await business.HttpClient.DeleteClassMaterialFileAsync(classGroup.Id);

        (await response.Content.ReadFromJsonAsync<ClassGroupResponse>(ApiRequests.JsonOptions))!.MaterialFile.ShouldBeNull();
        using var removedFile = await fixture.AnonymousClient.GetAsync(new Uri(file.Url));
        removedFile.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
