namespace ClassManager.Api.I.Tests.Endpoints.ClassMaterial.When_owner_adds_a_link_to_a_class_with_a_material_pdf;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_sees_both(ApiFixture fixture)
{
    private const string MaterialUrl = "https://dfswimmingteam.com/material/inicial.pdf";

    [Fact]
    public async Task Then_the_student_sees_both_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var instructors = scenario.Instructors;
        var classGroup = await instructors.Business.HttpClient.UploadClassMaterialFileAsync(instructors.InstructorClassGroup.Id);

        await instructors.Business.HttpClient.ShareClassGroupMaterialAsync(classGroup, MaterialUrl);

        var material = (await scenario.Student.GetStudentAppHomeAsync())!.Students.Single().Materials.Single();
        (material.LinkUrl, material.File?.Url).ShouldBe((MaterialUrl, classGroup.MaterialFile!.Url));
    }
}
