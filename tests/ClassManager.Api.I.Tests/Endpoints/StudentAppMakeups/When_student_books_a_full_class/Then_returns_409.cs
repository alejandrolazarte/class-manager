using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppMakeups.When_student_books_a_full_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var instructors = scenario.Instructors;
        var owner = instructors.Business.HttpClient;
        var fullClassGroup = await owner.CreateClassGroupAsync(
            ClassGroupRequests.ClassGroupFor(instructors.OtherInstructorId, [InstructorScenario.ClassDate.DayOfWeek], "20:00") with { Name = "Natación llena", Capacity = 1 });
        await owner.EnrollAsync(fullClassGroup.Id, instructors.OtherStudentId, InstructorScenario.ClassDate);
        (await scenario.Student.PutAbsenceAsync(instructors.InstructorStudentId, instructors.InstructorClassGroup.Id, InstructorScenario.ClassDate)).EnsureSuccessStatusCode();

        using var response = await scenario.Student.PutMakeupAsync(instructors.InstructorStudentId, fullClassGroup.Id, InstructorScenario.ClassDate);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await response.Content.ReadAsStringAsync()).ShouldContain(SessionErrorCodes.MakeupFull);
    }
}
