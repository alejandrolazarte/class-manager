using ClassManager.Core.Domain.Products;
using ClassManager.Core.UseCases.Products;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppShop.When_student_opens_the_shop;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_only_products_shown_in_the_app_are_listed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_only_products_shown_in_the_app_are_listed_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        await owner.CreateClassPackAsync();
        await owner.CreateProductAsync(StockMode.Unlimited);
        using (var hidden = await owner.PostAsJsonAsync(
            ApiRoutes.Products,
            new CreateProductCommand("Toalla del staff", null, 10m, StockMode.Unlimited, false, null),
            ApiRequests.JsonOptions))
        {
            hidden.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        var shop = await scenario.Student.GetStudentAppShopAsync();

        shop.Products.Select(product => product.Name).ShouldBe([ProductRequests.ProductName]);
        shop.Packs.Single().Name.ShouldBe(ClassPackRequests.PackName);
    }
}
