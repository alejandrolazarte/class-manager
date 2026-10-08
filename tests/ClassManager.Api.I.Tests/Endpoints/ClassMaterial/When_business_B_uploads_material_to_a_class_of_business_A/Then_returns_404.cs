namespace ClassManager.Api.I.Tests.Endpoints.ClassMaterial.When_business_B_uploads_material_to_a_class_of_business_A;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var businessA = await fixture.SeedBusinessAsync();
        var businessB = await fixture.SeedBusinessAsync();
        var instructor = await businessA.HttpClient.CreateInstructorAsync();
        var classGroupOfA = await businessA.HttpClient.CreateClassGroupAsync(ClassGroupRequests.ClassGroupFor(instructor.Id));

        using var response = await businessB.HttpClient.PostClassMaterialFileAsync(classGroupOfA.Id, ClassMaterialRequests.Pdf);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
