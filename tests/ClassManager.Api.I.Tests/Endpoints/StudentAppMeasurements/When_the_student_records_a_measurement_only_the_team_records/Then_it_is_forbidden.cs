namespace ClassManager.Api.I.Tests.Endpoints.StudentAppMeasurements.When_the_student_records_a_measurement_only_the_team_records;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_forbidden(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_forbidden_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var measurement = await scenario.Instructors.Business.HttpClient.CreateMeasurementAsync();

        using var response = await scenario.Student.PostStudentAppMeasurementRecordAsync(scenario.Instructors.InstructorStudentId, measurement.Id, 29.7m);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
