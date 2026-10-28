namespace ClassManager.Api.I.Tests.Endpoints.StudentAppFamily.When_a_teenager_opens_the_shop;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_cannot_buy(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_cannot_buy_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var clientEmail = StudentAppRequests.UniqueStudentEmail();
        var family = await business.HttpClient.RegisterFamilyAsync(clientEmail);
        using var teenager = await fixture.SignInFamilyMemberAsync(business.HttpClient, family.Id, "Lucía Pérez", StudentAppRequests.ChildBirthDate);

        var shop = await teenager.App.GetStudentAppShopAsync();

        shop.CanBuy.ShouldBeFalse();
    }
}
