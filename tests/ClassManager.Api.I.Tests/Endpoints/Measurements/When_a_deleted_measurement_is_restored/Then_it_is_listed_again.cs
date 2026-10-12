namespace ClassManager.Api.I.Tests.Endpoints.Measurements.When_a_deleted_measurement_is_restored;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_listed_again(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_listed_again_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var measurement = await business.HttpClient.CreateMeasurementAsync();
        (await business.HttpClient.DeleteMeasurementAsync(measurement.Id)).EnsureSuccessStatusCode();

        using var response = await business.HttpClient.RestoreMeasurementAsync(measurement.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await business.HttpClient.ListMeasurementsAsync())!.Single().Id.ShouldBe(measurement.Id);
    }
}
