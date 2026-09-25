using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_Enrollment_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherClassGroup = await otherBusiness.HttpClient.CreateClassGroupWithInstructorAsync();
        var otherEnrollment = await otherBusiness.HttpClient.EnrollAsync(otherClassGroup.Id, await otherBusiness.HttpClient.RegisterStudentAsync());

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.Enrollments.AnyAsync(enrollment => enrollment.Id == otherEnrollment.Id)).ShouldBeFalse();
    }
}
