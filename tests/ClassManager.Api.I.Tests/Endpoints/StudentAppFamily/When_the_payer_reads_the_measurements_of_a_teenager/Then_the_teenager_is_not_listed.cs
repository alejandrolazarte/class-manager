namespace ClassManager.Api.I.Tests.Endpoints.StudentAppFamily.When_the_payer_reads_the_measurements_of_a_teenager;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_teenager_is_not_listed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_teenager_is_not_listed_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var clientEmail = StudentAppRequests.UniqueStudentEmail();
        var family = await business.HttpClient.RegisterFamilyAsync(clientEmail);
        using var teenager = await fixture.SignInFamilyMemberAsync(business.HttpClient, family.Id, "Lucía Pérez", StudentAppRequests.ChildBirthDate);
        using var payerApp = await fixture.SignInClientOfFamilyAsync(business.HttpClient, family.Id, clientEmail);

        var measurements = await payerApp.GetStudentAppMeasurementsAsync();

        measurements!.Students.Select(student => student.StudentId).ShouldNotContain(teenager.Student.Id);
    }
}
