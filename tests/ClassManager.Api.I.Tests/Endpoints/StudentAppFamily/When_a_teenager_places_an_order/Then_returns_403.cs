namespace ClassManager.Api.I.Tests.Endpoints.StudentAppFamily.When_a_teenager_places_an_order;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var clientEmail = StudentAppRequests.UniqueStudentEmail();
        var family = await business.HttpClient.RegisterFamilyAsync(clientEmail);
        var classPack = await business.HttpClient.CreateClassPackAsync();
        using var teenager = await fixture.SignInFamilyMemberAsync(business.HttpClient, family.Id, "Lucía Pérez", StudentAppRequests.ChildBirthDate);

        using var response = await teenager.App.PostStudentAppOrderAsync(StudentAppShopRequests.PackLine(classPack.Id));

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
