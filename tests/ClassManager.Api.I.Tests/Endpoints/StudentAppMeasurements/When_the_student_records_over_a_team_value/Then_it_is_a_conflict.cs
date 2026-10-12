namespace ClassManager.Api.I.Tests.Endpoints.StudentAppMeasurements.When_the_student_records_over_a_team_value;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_a_conflict(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_a_conflict_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var measurement = await owner.CreateMeasurementAsync(MeasurementRequests.DistanceRecordedByStudents());
        await owner.RecordMeasurementAsync(scenario.Instructors.InstructorStudentId, measurement.Id, 300m);

        using var response = await scenario.Student.PostStudentAppMeasurementRecordAsync(scenario.Instructors.InstructorStudentId, measurement.Id, 400m);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
