using ClassManager.Core.UseCases.Enrollments;

namespace ClassManager.Api.I.Tests.Endpoints.Enrollments.When_ending_enrollment;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_student_leaves_the_roster_after_end_date(ApiFixture fixture)
{
    [Fact]
    public async Task Then_student_leaves_the_roster_after_end_date_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();
        var enrollment = await business.HttpClient.EnrollAsync(
            classGroup.Id, await business.HttpClient.RegisterStudentAsync(), EnrollmentRequests.Today.AddDays(-30));

        using var response = await business.HttpClient.PutAsJsonAsync(
            $"{ApiRoutes.Enrollments}/{enrollment.Id}{ApiRoutes.End}",
            new EndEnrollmentRequest(EnrollmentRequests.Today.AddDays(-1)),
            ApiRequests.JsonOptions);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await business.HttpClient.GetRosterAsync(classGroup.Id)).ShouldBeEmpty();
    }
}
