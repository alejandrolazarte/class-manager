using ClassManager.Core.UseCases.Enrollments;

namespace ClassManager.Api.I.Tests.Endpoints.Enrollments.When_listing_student_enrollments;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_class_details_are_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_class_details_are_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var classGroup = await business.HttpClient.CreateClassGroupWithInstructorAsync();
        var studentId = await business.HttpClient.RegisterStudentAsync();
        await business.HttpClient.EnrollAsync(classGroup.Id, studentId);

        var enrollments = await business.HttpClient.GetFromJsonAsync<List<StudentEnrollmentResponse>>(
            new Uri($"{ApiRoutes.Students}/{studentId}{ApiRoutes.EnrollmentsSegment}", UriKind.Relative), ApiRequests.JsonOptions);

        enrollments!.Single().ClassGroupName.ShouldBe(classGroup.Name);
        enrollments![0].StartTime.ShouldBe("18:00");
        enrollments![0].InstructorFullName.ShouldBe(ClassGroupRequests.InstructorFullName);
    }
}
