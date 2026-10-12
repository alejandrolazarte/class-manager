namespace ClassManager.Api.I.Tests.Endpoints.Measurements.When_an_instructor_records_a_student_of_another_instructor;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_forbidden(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_forbidden_Run()
    {
        var scenario = await fixture.SeedInstructorScenarioAsync();
        var measurement = await scenario.Business.HttpClient.CreateMeasurementAsync();

        using var response = await scenario.Instructor.PostMeasurementRecordAsync(scenario.OtherStudentId, measurement.Id, 29.7m);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
