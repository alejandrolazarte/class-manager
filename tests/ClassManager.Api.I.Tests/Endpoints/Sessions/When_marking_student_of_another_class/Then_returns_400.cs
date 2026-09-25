using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.Sessions.When_marking_student_of_another_class;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();
        var notEnrolledStudentId = await business.HttpClient.RegisterStudentAsync();

        using var response = await business.HttpClient.PutAttendanceAsync(
            classGroup.Id, EnrollmentRequests.Today, notEnrolledStudentId, AttendanceStatus.Present);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
