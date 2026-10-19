using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.Sessions.When_marking_the_whole_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_every_student_is_present(ApiFixture fixture)
{
    [Fact]
    public async Task Then_every_student_is_present_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();
        var firstStudentId = await business.HttpClient.RegisterStudentAsync();
        var secondStudentId = await business.HttpClient.RegisterStudentAsync("Mateo Gómez", "11 4000-2222");
        await business.HttpClient.EnrollAsync(classGroup.Id, firstStudentId);
        await business.HttpClient.EnrollAsync(classGroup.Id, secondStudentId);

        using var response = await business.HttpClient.PutClassAttendanceAsync(
            classGroup.Id,
            EnrollmentRequests.Today,
            [new StudentAttendance(firstStudentId, AttendanceStatus.Present), new StudentAttendance(secondStudentId, AttendanceStatus.Present)]);

        var session = await business.HttpClient.GetSessionAsync(classGroup.Id, EnrollmentRequests.Today);
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        session!.Students.Select(student => student.Status).ShouldBe([AttendanceStatus.Present, AttendanceStatus.Present]);
    }
}
