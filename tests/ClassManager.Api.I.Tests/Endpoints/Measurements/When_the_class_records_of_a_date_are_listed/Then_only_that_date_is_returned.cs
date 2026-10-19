using ClassManager.Core.UseCases.Measurements;

namespace ClassManager.Api.I.Tests.Endpoints.Measurements.When_the_class_records_of_a_date_are_listed;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_that_date_is_returned(ApiFixture fixture)
{
    private static readonly DateOnly ClassDate = new(2026, 3, 10);
    private static readonly DateOnly EarlierDate = new(2026, 3, 3);

    [Fact]
    public async Task Then_only_that_date_is_returned_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();
        var owner = scenario.Business.HttpClient;
        var measurement = await owner.CreateMeasurementAsync();
        await owner.RecordMeasurementAsync(scenario.OtherStudentId, measurement.Id, 35.1m, EarlierDate);
        await owner.RecordMeasurementAsync(scenario.OtherStudentId, measurement.Id, 33.4m, ClassDate);

        using var response = await owner.GetClassMeasurementRecordsAsync(ClassDate, [scenario.InstructorStudentId, scenario.OtherStudentId]);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var records = await response.Content.ReadFromJsonAsync<List<MeasurementRecordResponse>>(ApiRequests.JsonOptions);
        records!.Single().Value.ShouldBe(33.4m);
    }
}
