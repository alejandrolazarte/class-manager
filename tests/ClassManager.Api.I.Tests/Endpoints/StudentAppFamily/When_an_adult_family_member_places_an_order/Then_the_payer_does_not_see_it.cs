namespace ClassManager.Api.I.Tests.Endpoints.StudentAppFamily.When_an_adult_family_member_places_an_order;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_payer_does_not_see_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_payer_does_not_see_it_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var clientEmail = StudentAppRequests.UniqueStudentEmail();
        var family = await business.HttpClient.RegisterFamilyAsync(clientEmail);
        var classPack = await business.HttpClient.CreateClassPackAsync();
        using var adult = await fixture.SignInFamilyMemberAsync(business.HttpClient, family.Id, "Martín Pérez", StudentAppRequests.AdultMemberBirthDate);
        using var payerApp = await fixture.SignInClientOfFamilyAsync(business.HttpClient, family.Id, clientEmail);
        await adult.App.PlaceStudentAppOrderAsync(StudentAppShopRequests.PackLine(classPack.Id));

        var orders = await payerApp.ListStudentAppOrdersAsync();

        orders.ShouldBeEmpty();
    }
}
