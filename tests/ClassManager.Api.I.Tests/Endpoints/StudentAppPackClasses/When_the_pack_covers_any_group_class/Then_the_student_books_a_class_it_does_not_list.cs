namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPackClasses.When_the_pack_covers_any_group_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_books_a_class_it_does_not_list(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_student_books_a_class_it_does_not_list_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var classPack = await owner.CreateAnyGroupClassPackAsync();
        await owner.SellClassPackAsync(scenario.ClientId, classPack.Id);

        using var response = await scenario.Student.PutPackClassAsync(
            scenario.Instructors.InstructorStudentId, scenario.Instructors.OtherClassGroup.Id, InstructorScenario.ClassDate);

        response.IsSuccessStatusCode.ShouldBeTrue();
    }
}
