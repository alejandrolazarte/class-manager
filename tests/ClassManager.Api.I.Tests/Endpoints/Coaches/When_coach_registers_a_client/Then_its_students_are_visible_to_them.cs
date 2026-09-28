using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Coaches.When_coach_registers_a_client;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_students_are_visible_to_them(ApiFixture fixture)
{
    private const string NewStudentFullName = "Martina Ruiz";

    [Fact]
    public async Task Then_its_students_are_visible_to_them_Run()
    {
        var scenario = await fixture.SeedCoachScenarioAsync();
        var client = await scenario.Coach.RegisterClientAsync(phoneNumber: "11 9988-7766", students: [new NewStudent(NewStudentFullName, null, null)]);

        var students = await scenario.Coach.SearchStudentsAsync(NewStudentFullName);

        students!.Select(student => student.Id).ShouldBe([client.Students[0].Id]);
    }
}
