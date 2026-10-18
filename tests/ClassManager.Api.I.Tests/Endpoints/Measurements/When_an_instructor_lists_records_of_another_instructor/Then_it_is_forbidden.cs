namespace ClassManager.Api.I.Tests.Endpoints.Measurements.When_an_instructor_lists_records_of_another_instructor;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_forbidden(ApiFixture fixture)
{
    private static readonly DateOnly ClassDate = new(2026, 3, 10);

    [Fact]
    public async Task Then_it_is_forbidden_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();

        using var response = await scenario.Instructor.GetClassMeasurementRecordsAsync(ClassDate, [scenario.OtherStudentId]);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
