namespace ClassManager.Api.I.Tests.Endpoints.StudentAppFamily.When_the_payer_records_a_measurement_of_a_teenager;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var clientEmail = StudentAppRequests.UniqueStudentEmail();
        var family = await business.HttpClient.RegisterFamilyAsync(clientEmail);
        using var teenager = await fixture.SignInFamilyMemberAsync(business.HttpClient, family.Id, "Lucía Pérez", StudentAppRequests.ChildBirthDate);
        using var payerApp = await fixture.SignInClientOfFamilyAsync(business.HttpClient, family.Id, clientEmail);
        var measurement = await business.HttpClient.CreateMeasurementAsync(MeasurementRequests.DistanceRecordedByStudents());

        using var response = await payerApp.PostStudentAppMeasurementRecordAsync(teenager.Student.Id, measurement.Id, 25);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
