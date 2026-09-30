using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_ClassFeedback_belongs_to_another_business;

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
        (await otherBusiness.HttpClient.PutFeedbackAsync(otherClassGroup.Id, EnrollmentRequests.Today, otherStudentId, "Buena clase"))
            .EnsureSuccessStatusCode();

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.ClassFeedbacks.AnyAsync(feedback => feedback.StudentId == otherStudentId)).ShouldBeFalse();
    }
}
