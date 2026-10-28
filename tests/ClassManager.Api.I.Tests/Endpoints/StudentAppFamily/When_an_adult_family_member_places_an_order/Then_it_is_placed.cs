namespace ClassManager.Api.I.Tests.Endpoints.StudentAppFamily.When_an_adult_family_member_places_an_order;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_placed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_placed_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var clientEmail = StudentAppRequests.UniqueStudentEmail();
        var family = await business.HttpClient.RegisterFamilyAsync(clientEmail);
        var classPack = await business.HttpClient.CreateClassPackAsync();
        using var adult = await fixture.SignInFamilyMemberAsync(business.HttpClient, family.Id, "Martín Pérez", StudentAppRequests.AdultMemberBirthDate);

        using var response = await adult.App.PostStudentAppOrderAsync(StudentAppShopRequests.PackLine(classPack.Id));

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }
}
