using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.Sessions.When_marking_attendance;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_session_shows_status_and_day_counts(ApiFixture fixture)
{
    [Fact]
    public async Task Then_session_shows_status_and_day_counts_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();
        var studentId = await business.HttpClient.RegisterStudentAsync();
        await business.HttpClient.EnrollAsync(classGroup.Id, studentId);

        using var response = await business.HttpClient.PutAttendanceAsync(classGroup.Id, EnrollmentRequests.Today, studentId, AttendanceStatus.Present);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var session = await business.HttpClient.GetSessionAsync(classGroup.Id, EnrollmentRequests.Today);
        session!.Students.Single().Status.ShouldBe(AttendanceStatus.Present);
        var day = await business.HttpClient.GetDayAsync(EnrollmentRequests.Today);
        day!.Single().PresentCount.ShouldBe(1);
        day![0].EnrolledCount.ShouldBe(1);
    }
}
