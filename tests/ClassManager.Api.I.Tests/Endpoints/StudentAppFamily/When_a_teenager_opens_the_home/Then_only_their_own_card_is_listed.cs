namespace ClassManager.Api.I.Tests.Endpoints.StudentAppFamily.When_a_teenager_opens_the_home;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_their_own_card_is_listed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_only_their_own_card_is_listed_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var clientEmail = StudentAppRequests.UniqueStudentEmail();
        var family = await business.HttpClient.RegisterFamilyAsync(clientEmail);
        using var teenager = await fixture.SignInFamilyMemberAsync(business.HttpClient, family.Id, "Lucía Pérez", StudentAppRequests.ChildBirthDate);

        var home = await teenager.App.GetStudentAppHomeAsync();

        home!.Students.Select(student => student.FullName).ShouldBe(["Lucía Pérez"]);
    }
}
