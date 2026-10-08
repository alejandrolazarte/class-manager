using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppAbsences.When_student_warns_after_the_class_started;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var earlyClassGroup = await owner.CreateClassGroupAsync(
            ClassGroupRequests.ClassGroupFor(scenario.Instructors.InstructorId, [InstructorScenario.ClassDate.DayOfWeek], "08:00") with { Name = "Natación temprano" });
        await owner.EnrollAsync(earlyClassGroup.Id, scenario.Instructors.InstructorStudentId, InstructorScenario.ClassDate);

        using var response = await scenario.Student.PutAbsenceAsync(scenario.Instructors.InstructorStudentId, earlyClassGroup.Id, InstructorScenario.ClassDate);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await response.Content.ReadAsStringAsync()).ShouldContain(SessionErrorCodes.ClassStarted);
    }
}
