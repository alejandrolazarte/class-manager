namespace ClassManager.Api.I.Tests.Endpoints.StudentAppFamily.When_the_payer_opens_the_home_with_an_adult_in_the_family;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_adult_is_not_listed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_adult_is_not_listed_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var clientEmail = StudentAppRequests.UniqueStudentEmail();
        var family = await business.HttpClient.RegisterFamilyAsync(clientEmail);
        using var adult = await fixture.SignInFamilyMemberAsync(business.HttpClient, family.Id, "Martín Pérez", StudentAppRequests.AdultMemberBirthDate);
        using var payerApp = await fixture.SignInClientOfFamilyAsync(business.HttpClient, family.Id, clientEmail);

        var home = await payerApp.GetStudentAppHomeAsync();

        home!.Students.Select(student => student.FullName).ShouldBe([StudentAppRequests.ChildFullName]);
    }
}
