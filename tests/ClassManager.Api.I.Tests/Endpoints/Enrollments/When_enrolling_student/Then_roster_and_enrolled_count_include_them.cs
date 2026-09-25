using ClassManager.Core.UseCases.ClassGroups;

namespace ClassManager.Api.I.Tests.Endpoints.Enrollments.When_enrolling_student;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_roster_and_enrolled_count_include_them(ApiFixture fixture)
{
    [Fact]
    public async Task Then_roster_and_enrolled_count_include_them_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();
        var studentId = await business.HttpClient.RegisterStudentAsync();

        using var response = await business.HttpClient.PostEnrollmentAsync(classGroup.Id, studentId);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var roster = await business.HttpClient.GetRosterAsync(classGroup.Id);
        roster!.Select(entry => entry.StudentId).ShouldBe([studentId]);
        roster![0].StartDate.ShouldBe(EnrollmentRequests.Today);
        var classGroups = await business.HttpClient.GetFromJsonAsync<List<ClassGroupResponse>>(
            new Uri(ApiRoutes.ClassGroups, UriKind.Relative), ApiRequests.JsonOptions);
        classGroups!.Single().EnrolledCount.ShouldBe(1);
    }
}
