namespace ClassManager.Api.I.Tests.Endpoints.StudentAppMeasurements.When_the_student_records_another_family_student;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_found(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_found_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var measurement = await scenario.Instructors.Business.HttpClient.CreateMeasurementAsync(MeasurementRequests.DistanceRecordedByStudents());

        using var response = await scenario.Student.PostStudentAppMeasurementRecordAsync(scenario.Instructors.OtherStudentId, measurement.Id, 400m);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
