namespace ClassManager.Api.I.Tests.Endpoints.ClassMaterial.When_owner_uploads_a_file_that_is_not_a_pdf;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var instructor = await business.HttpClient.CreateInstructorAsync();
        var classGroup = await business.HttpClient.CreateClassGroupAsync(ClassGroupRequests.ClassGroupFor(instructor.Id));

        using var response = await business.HttpClient.PostClassMaterialFileAsync(classGroup.Id, CatalogImageRequests.PngImage);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
