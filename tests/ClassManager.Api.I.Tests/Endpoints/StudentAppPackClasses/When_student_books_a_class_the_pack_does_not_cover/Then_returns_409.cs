using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPackClasses.When_student_books_a_class_the_pack_does_not_cover;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var scenario = await fixture.SeedPackStudentAppScenarioAsync();
        var otherClassGroup = await scenario.Owner.CreateClassGroupAsync(
            ClassGroupRequests.ClassGroupFor(scenario.Instructors.OtherInstructorId, [InstructorScenario.ClassDate.DayOfWeek], "20:00") with { Name = "Aquagym" });

        using var response = await scenario.Student.PutPackClassAsync(scenario.StudentId, otherClassGroup.Id, InstructorScenario.ClassDate);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await response.Content.ReadAsStringAsync()).ShouldContain(SessionErrorCodes.PackNoClasses);
    }
}
