namespace ClassManager.Api.I.Tests.Endpoints.Measurements.When_a_deleted_record_is_restored;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_has_it_again(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_student_has_it_again_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var studentId = await business.HttpClient.RegisterStudentAsync();
        var measurement = await business.HttpClient.CreateMeasurementAsync();
        var record = await business.HttpClient.RecordMeasurementAsync(studentId, measurement.Id, 29.7m);
        (await business.HttpClient.DeleteMeasurementRecordAsync(record.Id)).EnsureSuccessStatusCode();

        using var response = await business.HttpClient.RestoreMeasurementRecordAsync(record.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await business.HttpClient.GetStudentMeasurementsAsync(studentId))!.Single().Records.Single().Id.ShouldBe(record.Id);
    }
}
