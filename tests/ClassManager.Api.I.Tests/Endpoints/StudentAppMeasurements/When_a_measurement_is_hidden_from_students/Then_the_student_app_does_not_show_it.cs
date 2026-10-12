namespace ClassManager.Api.I.Tests.Endpoints.StudentAppMeasurements.When_a_measurement_is_hidden_from_students;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_app_does_not_show_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_student_app_does_not_show_it_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var hidden = await owner.CreateMeasurementAsync(MeasurementRequests.TimeMeasurement(isVisibleToStudents: false));
        await owner.RecordMeasurementAsync(scenario.Instructors.InstructorStudentId, hidden.Id, 29.7m);

        var measurements = await scenario.Student.GetStudentAppMeasurementsAsync();

        measurements!.Students.Single().Measurements.ShouldBeEmpty();
    }
}
