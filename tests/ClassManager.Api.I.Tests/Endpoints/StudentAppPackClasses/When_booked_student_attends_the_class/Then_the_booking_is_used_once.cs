using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPackClasses.When_booked_student_attends_the_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_booking_is_used_once(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_booking_is_used_once_Run()
    {
        var scenario = await fixture.SeedPackStudentAppScenarioAsync();
        await scenario.BookPackClassAsync();

        (await scenario.Owner.PutAttendanceAsync(
            scenario.PackClassGroupId, CoachScenario.ClassDate, scenario.StudentId, AttendanceStatus.Present)).EnsureSuccessStatusCode();

        (await scenario.Student.GetPackClassesAsync(scenario.StudentId))!.ClassesLeft.ShouldBe(3);
    }
}
