using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.Students.When_a_student_who_took_classes_is_deleted;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_past_class_can_still_be_marked(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_past_class_can_still_be_marked_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var lastWeek = EnrollmentRequests.Today.AddDays(-7);
        var instructor = await owner.CreateInstructorAsync();
        var classGroup = await owner.CreateClassGroupAsync(ClassGroupRequests.ClassGroupFor(instructor.Id, [lastWeek.DayOfWeek]));
        var studentId = await owner.RegisterStudentAsync();
        await owner.EnrollAsync(classGroup.Id, studentId, lastWeek);
        (await owner.DeleteStudentAsync(studentId)).EnsureSuccessStatusCode();

        using var response = await owner.PutClassAttendanceAsync(classGroup.Id, lastWeek, [new StudentAttendance(studentId, AttendanceStatus.Present)]);

        response.IsSuccessStatusCode.ShouldBeTrue();
    }
}
