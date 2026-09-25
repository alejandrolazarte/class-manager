using ClassManager.Core.Domain.Sessions;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_Attendance_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherClassGroup = await otherBusiness.HttpClient.CreateClassGroupWithInstructorAsync();
        var otherStudentId = await otherBusiness.HttpClient.RegisterStudentAsync();
        await otherBusiness.HttpClient.EnrollAsync(otherClassGroup.Id, otherStudentId);
        (await otherBusiness.HttpClient.PutAttendanceAsync(otherClassGroup.Id, EnrollmentRequests.Today, otherStudentId, AttendanceStatus.Absent)).EnsureSuccessStatusCode();

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.Attendances.AnyAsync(attendance => attendance.StudentId == otherStudentId)).ShouldBeFalse();
    }
}
