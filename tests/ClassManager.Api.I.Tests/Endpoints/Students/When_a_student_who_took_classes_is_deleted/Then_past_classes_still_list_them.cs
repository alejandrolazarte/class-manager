using ClassManager.Core.Domain.Sessions;

namespace ClassManager.Api.I.Tests.Endpoints.Students.When_a_student_who_took_classes_is_deleted;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_past_classes_still_list_them(ApiFixture fixture)
{
    [Fact]
    public async Task Then_past_classes_still_list_them_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var lastWeek = EnrollmentRequests.Today.AddDays(-7);
        var instructor = await owner.CreateInstructorAsync();
        var classGroup = await owner.CreateClassGroupAsync(ClassGroupRequests.ClassGroupFor(instructor.Id, [lastWeek.DayOfWeek]));
        var studentId = await owner.RegisterStudentAsync();
        await owner.EnrollAsync(classGroup.Id, studentId, lastWeek);
        (await owner.PutAttendanceAsync(classGroup.Id, lastWeek, studentId, AttendanceStatus.Present)).EnsureSuccessStatusCode();

        (await owner.DeleteStudentAsync(studentId)).EnsureSuccessStatusCode();

        (await owner.GetSessionAsync(classGroup.Id, lastWeek))!.Students.Select(student => student.StudentId).ShouldContain(studentId);
    }
}
