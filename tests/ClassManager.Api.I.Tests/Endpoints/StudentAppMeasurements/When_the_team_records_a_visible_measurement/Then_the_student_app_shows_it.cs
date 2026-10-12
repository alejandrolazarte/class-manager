namespace ClassManager.Api.I.Tests.Endpoints.StudentAppMeasurements.When_the_team_records_a_visible_measurement;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_app_shows_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_student_app_shows_it_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var measurement = await owner.CreateMeasurementAsync();
        await owner.RecordMeasurementAsync(scenario.Instructors.InstructorStudentId, measurement.Id, 29.7m);

        var measurements = await scenario.Student.GetStudentAppMeasurementsAsync();

        measurements!.Students.Single().Measurements.Single().Records.Single().Value.ShouldBe(29.7m);
    }
}
