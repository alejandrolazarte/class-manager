namespace ClassManager.Api.I.Tests.Endpoints.ClassMaterial.When_owner_replaces_the_material_file_with_a_link;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_old_file_is_deleted(ApiFixture fixture)
{
    private const string MaterialUrl = "https://dfswimmingteam.com/material/inicial.pdf";

    [Fact]
    public async Task Then_the_old_file_is_deleted_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var instructor = await business.HttpClient.CreateInstructorAsync();
        var classGroup = await business.HttpClient.CreateClassGroupAsync(ClassGroupRequests.ClassGroupFor(instructor.Id));
        var file = (await business.HttpClient.UploadClassMaterialFileAsync(classGroup.Id)).MaterialFile!;

        await business.HttpClient.ShareClassGroupMaterialAsync(classGroup, MaterialUrl);

        using var removedFile = await fixture.AnonymousClient.GetAsync(new Uri(file.Url));
        removedFile.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
