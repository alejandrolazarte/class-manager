using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Instructors.When_instructor_registers_a_client;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_students_are_visible_to_them(ApiFixture fixture)
{
    private const string NewStudentFullName = "Martina Ruiz";

    [Fact]
    public async Task Then_its_students_are_visible_to_them_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();
        var client = await scenario.Instructor.RegisterClientAsync(phoneNumber: "11 9988-7766", students: [new NewStudent(NewStudentFullName, null, null)]);

        var students = await scenario.Instructor.SearchStudentsAsync(NewStudentFullName);

        students!.Select(student => student.Id).ShouldBe([client.Students[0].Id]);
    }
}
