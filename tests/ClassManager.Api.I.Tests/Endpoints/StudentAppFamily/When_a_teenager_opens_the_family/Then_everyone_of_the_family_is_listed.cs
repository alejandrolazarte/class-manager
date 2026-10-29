namespace ClassManager.Api.I.Tests.Endpoints.StudentAppFamily.When_a_teenager_opens_the_family;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_everyone_of_the_family_is_listed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_everyone_of_the_family_is_listed_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var clientEmail = StudentAppRequests.UniqueStudentEmail();
        var family = await business.HttpClient.RegisterFamilyAsync(clientEmail);
        using var teenager = await fixture.SignInFamilyMemberAsync(business.HttpClient, family.Id, "Lucía Pérez", StudentAppRequests.ChildBirthDate);

        var members = (await teenager.App.GetStudentAppFamilyAsync())!.Members;

        members.Select(member => member.FullName).ShouldBe([ApiRequests.ClientFullName, StudentAppRequests.ChildFullName, "Lucía Pérez"], ignoreOrder: true);
    }
}
