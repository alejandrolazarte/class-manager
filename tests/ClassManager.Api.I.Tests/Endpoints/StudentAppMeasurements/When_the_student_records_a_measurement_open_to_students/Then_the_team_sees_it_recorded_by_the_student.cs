namespace ClassManager.Api.I.Tests.Endpoints.StudentAppMeasurements.When_the_student_records_a_measurement_open_to_students;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_team_sees_it_recorded_by_the_student(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_team_sees_it_recorded_by_the_student_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var measurement = await owner.CreateMeasurementAsync(MeasurementRequests.DistanceRecordedByStudents());

        using var response = await scenario.Student.PostStudentAppMeasurementRecordAsync(scenario.Instructors.InstructorStudentId, measurement.Id, 400m);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await owner.GetStudentMeasurementsAsync(scenario.Instructors.InstructorStudentId))!.Single().Records.Single().RecordedByStudent.ShouldBeTrue();
    }
}
