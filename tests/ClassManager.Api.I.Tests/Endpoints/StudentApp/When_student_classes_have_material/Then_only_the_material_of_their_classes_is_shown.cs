using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_student_classes_have_material;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_the_material_of_their_classes_is_shown(ApiFixture fixture)
{
    private const string StudentClassMaterialUrl = "https://dfswimmingteam.com/material/marcos.pdf";
    private const string OtherClassMaterialUrl = "https://dfswimmingteam.com/material/laura.pdf";

    [Fact]
    public async Task Then_only_the_material_of_their_classes_is_shown_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var coaches = scenario.Coaches;
        await coaches.Business.HttpClient.ShareClassGroupMaterialAsync(coaches.CoachClassGroup, StudentClassMaterialUrl);
        await coaches.Business.HttpClient.ShareClassGroupMaterialAsync(coaches.OtherClassGroup, OtherClassMaterialUrl);

        var home = await scenario.Student.GetStudentAppHomeAsync();

        home!.Students.Single().Materials.ShouldBe(
            [new StudentAppClassMaterialResponse(coaches.CoachClassGroup.Id, CoachScenario.CoachClassGroupName, StudentClassMaterialUrl)]);
    }
}
