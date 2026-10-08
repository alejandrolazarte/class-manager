namespace ClassManager.Api.I.Tests.Endpoints.ClassMaterial.When_owner_uploads_a_class_material_pdf;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_opens_it_from_its_url(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_student_opens_it_from_its_url_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var instructors = scenario.Instructors;
        await instructors.Business.HttpClient.UploadClassMaterialFileAsync(instructors.InstructorClassGroup.Id);

        var home = await scenario.Student.GetStudentAppHomeAsync();

        var material = home!.Students.Single().Materials.Single();
        using var response = await fixture.AnonymousClient.GetAsync(new Uri(material.Url));
        (await response.Content.ReadAsByteArrayAsync()).ShouldBe(ClassMaterialRequests.Pdf);
    }
}
