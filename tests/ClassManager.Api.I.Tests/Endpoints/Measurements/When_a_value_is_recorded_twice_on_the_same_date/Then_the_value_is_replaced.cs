namespace ClassManager.Api.I.Tests.Endpoints.Measurements.When_a_value_is_recorded_twice_on_the_same_date;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_value_is_replaced(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_value_is_replaced_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var studentId = await business.HttpClient.RegisterStudentAsync();
        var measurement = await business.HttpClient.CreateMeasurementAsync();
        await business.HttpClient.RecordMeasurementAsync(studentId, measurement.Id, 30.9m);

        await business.HttpClient.RecordMeasurementAsync(studentId, measurement.Id, 29.7m);

        var measurements = await business.HttpClient.GetStudentMeasurementsAsync(studentId);
        measurements!.Single().Records.Single().Value.ShouldBe(29.7m);
    }
}
