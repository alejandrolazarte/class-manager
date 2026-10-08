using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_AbsenceNotice_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherStudent = await fixture.SeedStudentAppScenarioAsync();
        (await otherStudent.Student.PutAbsenceAsync(
            otherStudent.Instructors.InstructorStudentId, otherStudent.Instructors.InstructorClassGroup.Id, InstructorScenario.ClassDate)).EnsureSuccessStatusCode();

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.AbsenceNotices.AnyAsync(notice => notice.StudentId == otherStudent.Instructors.InstructorStudentId)).ShouldBeFalse();
    }
}
