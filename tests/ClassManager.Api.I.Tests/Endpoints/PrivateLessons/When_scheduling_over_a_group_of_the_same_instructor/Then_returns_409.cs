namespace ClassManager.Api.I.Tests.Endpoints.PrivateLessons.When_scheduling_over_a_group_of_the_same_instructor;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();
        var studentId = await business.HttpClient.RegisterStudentAsync();

        using var response = await business.HttpClient.PostPrivateLessonAsync(
            classGroup.InstructorId, studentId, EnrollmentRequests.Today, classGroup.StartTime);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
