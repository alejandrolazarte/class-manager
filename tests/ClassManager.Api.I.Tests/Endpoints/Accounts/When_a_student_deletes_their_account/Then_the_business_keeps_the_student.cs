namespace ClassManager.Api.I.Tests.Endpoints.Accounts.When_a_student_deletes_their_account;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_business_keeps_the_student(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_business_keeps_the_student_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();

        await scenario.DeleteAccountAsync();

        var students = await scenario.Instructors.Business.HttpClient.SearchStudentsAsync(InstructorScenario.InstructorStudentFullName);
        students!.ShouldContain(student => student.Id == scenario.Instructors.InstructorStudentId);
    }
}
