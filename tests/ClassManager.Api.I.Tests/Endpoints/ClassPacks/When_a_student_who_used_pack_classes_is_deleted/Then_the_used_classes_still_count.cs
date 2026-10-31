using ClassManager.Core.Domain.Sessions;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.ClassPacks.When_a_student_who_used_pack_classes_is_deleted;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_used_classes_still_count(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_used_classes_still_count_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var owner = business.HttpClient;
        var classGroup = await owner.CreateClassGroupWithInstructorAsync();
        var client = await owner.RegisterClientAsync(students: [new NewStudent(ApiRequests.StudentFullName, null, null), new NewStudent("Martina Pérez", null, null)]);
        var studentId = client.Students.Single(student => student.FullName == ApiRequests.StudentFullName).Id;
        await owner.EnrollAsync(classGroup.Id, studentId);
        var classPack = await owner.CreateClassPackAsync(classGroupIds: [classGroup.Id]);
        await owner.SellClassPackAsync(client.Id, classPack.Id);
        (await owner.PutAttendanceAsync(classGroup.Id, EnrollmentRequests.Today, studentId, AttendanceStatus.Present)).EnsureSuccessStatusCode();

        (await owner.DeleteStudentAsync(studentId)).EnsureSuccessStatusCode();

        (await owner.GetClassBalanceAsync(client.Id))!.AvailableClasses.ShouldBe(3);
    }
}
