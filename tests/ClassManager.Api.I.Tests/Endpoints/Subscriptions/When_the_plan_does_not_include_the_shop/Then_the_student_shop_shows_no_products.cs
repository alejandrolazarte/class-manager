using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_the_plan_does_not_include_the_shop;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_shop_shows_no_products(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_student_shop_shows_no_products_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        await owner.RestockAsync(await owner.CreateProductAsync(), 2);
        var classPack = await owner.CreateClassPackAsync();
        await fixture.ChangePlanAsync(scenario.Instructors.Business.Business.Id, PlanCodes.Lite);

        var shop = await scenario.Student.GetStudentAppShopAsync();

        shop.Products.ShouldBeEmpty();
        shop.Packs.Single().Id.ShouldBe(classPack.Id);
    }
}
