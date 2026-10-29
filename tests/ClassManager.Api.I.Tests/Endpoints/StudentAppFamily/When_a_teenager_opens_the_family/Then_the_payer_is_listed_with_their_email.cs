namespace ClassManager.Api.I.Tests.Endpoints.StudentAppFamily.When_a_teenager_opens_the_family;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_payer_is_listed_with_their_email(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_payer_is_listed_with_their_email_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var clientEmail = StudentAppRequests.UniqueStudentEmail();
        var family = await business.HttpClient.RegisterFamilyAsync(clientEmail);
        using var teenager = await fixture.SignInFamilyMemberAsync(business.HttpClient, family.Id, "Lucía Pérez", StudentAppRequests.ChildBirthDate);

        var members = (await teenager.App.GetStudentAppFamilyAsync())!.Members;

        members.Single(member => member.PaysForFamily).Email.ShouldBe(clientEmail);
    }
}
