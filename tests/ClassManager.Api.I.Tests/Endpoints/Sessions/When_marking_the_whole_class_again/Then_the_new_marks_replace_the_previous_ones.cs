using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.Sessions.When_marking_the_whole_class_again;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_new_marks_replace_the_previous_ones(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_new_marks_replace_the_previous_ones_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();
        var firstStudentId = await business.HttpClient.RegisterStudentAsync();
        var secondStudentId = await business.HttpClient.RegisterStudentAsync("Mateo Gómez", "11 4000-2222");
        await business.HttpClient.EnrollAsync(classGroup.Id, firstStudentId);
        await business.HttpClient.EnrollAsync(classGroup.Id, secondStudentId);
        using var firstMarks = await business.HttpClient.PutClassAttendanceAsync(
            classGroup.Id,
            EnrollmentRequests.Today,
            [new StudentAttendance(firstStudentId, AttendanceStatus.Present), new StudentAttendance(secondStudentId, AttendanceStatus.Present)]);

        using var response = await business.HttpClient.PutClassAttendanceAsync(
            classGroup.Id,
            EnrollmentRequests.Today,
            [new StudentAttendance(firstStudentId, AttendanceStatus.Absent), new StudentAttendance(secondStudentId, null)]);

        var session = await business.HttpClient.GetSessionAsync(classGroup.Id, EnrollmentRequests.Today);
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        session!.Students.ToDictionary(student => student.StudentId, student => student.Status)
            .ShouldBe(new Dictionary<long, AttendanceStatus?> { [firstStudentId] = AttendanceStatus.Absent, [secondStudentId] = null }, ignoreOrder: true);
    }
}
