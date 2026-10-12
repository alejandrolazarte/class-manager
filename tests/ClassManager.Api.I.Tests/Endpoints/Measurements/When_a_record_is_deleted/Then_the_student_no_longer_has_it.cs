namespace ClassManager.Api.I.Tests.Endpoints.Measurements.When_a_record_is_deleted;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_no_longer_has_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_student_no_longer_has_it_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var studentId = await business.HttpClient.RegisterStudentAsync();
        var measurement = await business.HttpClient.CreateMeasurementAsync();
        var record = await business.HttpClient.RecordMeasurementAsync(studentId, measurement.Id, 29.7m);

        using var response = await business.HttpClient.DeleteMeasurementRecordAsync(record.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await business.HttpClient.GetStudentMeasurementsAsync(studentId))!.Single().Records.ShouldBeEmpty();
    }
}
