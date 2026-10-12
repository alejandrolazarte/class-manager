namespace ClassManager.Api.I.Tests.Endpoints.Measurements.When_a_measurement_is_created;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_listed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_listed_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        var measurement = await business.HttpClient.CreateMeasurementAsync();

        (await business.HttpClient.ListMeasurementsAsync())!.Single().ShouldBe(measurement);
    }
}
