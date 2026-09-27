using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.PrivateLessons.When_listing_a_day_with_a_private_lesson;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_listed_with_its_student(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_listed_with_its_student_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var instructor = await business.HttpClient.CreateInstructorAsync();
        var studentId = await business.HttpClient.RegisterStudentAsync();
        var lesson = await business.HttpClient.SchedulePrivateLessonAsync(instructor.Id, studentId, EnrollmentRequests.Today);

        var day = await business.HttpClient.GetDayAsync(EnrollmentRequests.Today);

        var privateSession = day!.Single(session => session.Kind == SessionKind.Private);
        privateSession.PrivateLessonId.ShouldBe(lesson.Id);
        privateSession.StudentNames.ShouldBe([ApiRequests.StudentFullName]);
    }
}
