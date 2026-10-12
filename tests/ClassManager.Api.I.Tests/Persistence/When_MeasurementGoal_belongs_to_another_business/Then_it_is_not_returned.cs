using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_MeasurementGoal_belongs_to_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherStudentId = await otherBusiness.HttpClient.RegisterStudentAsync();
        var otherMeasurement = await otherBusiness.HttpClient.CreateMeasurementAsync();
        (await otherBusiness.HttpClient.PutMeasurementGoalAsync(otherStudentId, otherMeasurement.Id, 28m)).EnsureSuccessStatusCode();

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.MeasurementGoals.AnyAsync(goal => goal.StudentId == otherStudentId)).ShouldBeFalse();
    }
}
