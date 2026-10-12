namespace ClassManager.Api.I.Tests.Endpoints.Measurements.When_the_owner_records_a_value;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_record_names_the_owner(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_record_names_the_owner_Run()
    {
        using var anonymousClient = fixture.ApiFactory.CreateClient();
        var tokens = await anonymousClient.SignUpAsync(fixture.ApiFactory.EmailTransport, AuthenticationRequests.SignUpCommand());
        using var owner = fixture.CreateClientWithToken(tokens.AccessToken);
        var studentId = await owner.RegisterStudentAsync();
        var measurement = await owner.CreateMeasurementAsync();
        await owner.RecordMeasurementAsync(studentId, measurement.Id, 29.7m);

        var measurements = await owner.GetStudentMeasurementsAsync(studentId);

        measurements!.Single().Records.Single().RecordedByFullName.ShouldBe(AuthenticationRequests.OwnerFullName);
    }
}
