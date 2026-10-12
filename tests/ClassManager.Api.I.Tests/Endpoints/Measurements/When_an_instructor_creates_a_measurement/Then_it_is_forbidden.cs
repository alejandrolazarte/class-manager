namespace ClassManager.Api.I.Tests.Endpoints.Measurements.When_an_instructor_creates_a_measurement;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_forbidden(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_forbidden_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        using var response = await scenario.Instructor.PostMeasurementAsync(MeasurementRequests.TimeMeasurement());

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
