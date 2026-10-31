namespace ClassManager.Api.I.Tests.Endpoints.StudentAppShop.When_the_business_offers_a_plan_that_renews_monthly;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_shop_leaves_it_out(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_shop_leaves_it_out_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var pack = await owner.CreateClassPackAsync();
        await owner.CreateMonthlyPlanAsync();

        var shop = await scenario.Student.GetStudentAppShopAsync();

        shop.Packs.Select(shopPack => shopPack.Id).ShouldBe([pack.Id]);
    }
}
