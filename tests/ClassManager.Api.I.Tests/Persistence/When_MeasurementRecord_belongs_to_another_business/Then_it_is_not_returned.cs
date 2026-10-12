using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_MeasurementRecord_belongs_to_another_business;

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
        var otherRecord = await otherBusiness.HttpClient.RecordMeasurementAsync(otherStudentId, otherMeasurement.Id, 29.7m);

        await using var context = fixture.CreateDbContext(business.Business.Id);

        (await context.MeasurementRecords.AnyAsync(record => record.Id == otherRecord.Id)).ShouldBeFalse();
    }
}
