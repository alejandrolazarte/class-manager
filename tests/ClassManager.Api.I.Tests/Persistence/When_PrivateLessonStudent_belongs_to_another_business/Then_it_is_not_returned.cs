using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_PrivateLessonStudent_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherInstructor = await otherBusiness.HttpClient.CreateInstructorAsync();
        var otherStudentId = await otherBusiness.HttpClient.RegisterStudentAsync();
        var otherLesson = await otherBusiness.HttpClient.SchedulePrivateLessonAsync(otherInstructor.Id, otherStudentId, EnrollmentRequests.Today);

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.PrivateLessonStudents.AnyAsync(lessonStudent => lessonStudent.PrivateLessonId == otherLesson.Id)).ShouldBeFalse();
    }
}
