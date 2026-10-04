using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_MakeupBooking_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherStudent = await fixture.SeedStudentAppScenarioAsync();
        await otherStudent.NoticeAndBookOtherClassAsync();

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.MakeupBookings.AnyAsync(booking => booking.StudentId == otherStudent.Coaches.CoachStudentId)).ShouldBeFalse();
    }
}
