namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPackClasses.When_student_cancels_a_pack_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_class_is_given_back(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_class_is_given_back_Run()
    {
        var scenario = await fixture.SeedPackStudentAppScenarioAsync();
        await scenario.BookPackClassAsync();

        using var response = await scenario.Student.DeletePackClassAsync(scenario.StudentId, scenario.PackClassGroupId, CoachScenario.ClassDate);

        response.EnsureSuccessStatusCode();
        (await scenario.Student.GetPackClassesAsync(scenario.StudentId))!.ClassesLeft.ShouldBe(4);
    }
}
