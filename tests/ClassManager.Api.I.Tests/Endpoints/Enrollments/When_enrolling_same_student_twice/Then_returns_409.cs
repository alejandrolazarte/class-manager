namespace ClassManager.Api.I.Tests.Endpoints.Enrollments.When_enrolling_same_student_twice;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();
        var studentId = await business.HttpClient.RegisterStudentAsync();
        await business.HttpClient.EnrollAsync(classGroup.Id, studentId);

        using var response = await business.HttpClient.PostEnrollmentAsync(classGroup.Id, studentId);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
