using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_the_plan_does_not_include_the_shop;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_a_student_cannot_order_products(ApiFixture fixture)
{
    [Fact]
    public async Task Then_a_student_cannot_order_products_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 2);
        await fixture.ChangePlanAsync(scenario.Instructors.Business.Business.Id, PlanCodes.Lite);

        using var response = await scenario.Student.PostStudentAppOrderAsync(StudentAppShopRequests.ProductLine(product.Variants[0].Id, 1));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
