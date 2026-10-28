namespace ClassManager.Api.I.Tests.Endpoints.StudentAppFamily.When_the_payer_opens_the_home_with_a_teenager;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_teenager_is_listed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_teenager_is_listed_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var clientEmail = StudentAppRequests.UniqueStudentEmail();
        var family = await business.HttpClient.RegisterFamilyAsync(clientEmail);
        using var teenager = await fixture.SignInFamilyMemberAsync(business.HttpClient, family.Id, "Lucía Pérez", StudentAppRequests.ChildBirthDate);
        using var payerApp = await fixture.SignInClientOfFamilyAsync(business.HttpClient, family.Id, clientEmail);

        var home = await payerApp.GetStudentAppHomeAsync();

        home!.Students.Select(student => student.FullName).ShouldContain("Lucía Pérez");
    }
}
