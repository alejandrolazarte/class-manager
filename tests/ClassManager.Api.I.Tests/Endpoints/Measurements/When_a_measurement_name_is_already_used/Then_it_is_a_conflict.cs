namespace ClassManager.Api.I.Tests.Endpoints.Measurements.When_a_measurement_name_is_already_used;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_a_conflict(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_a_conflict_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await business.HttpClient.CreateMeasurementAsync();

        using var response = await business.HttpClient.PostMeasurementAsync(MeasurementRequests.TimeMeasurement());

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
