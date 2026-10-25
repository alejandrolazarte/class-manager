using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_the_plan_does_not_include_the_shop;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_an_order_with_products_is_not_handed_over(ApiFixture fixture)
{
    [Fact]
    public async Task Then_an_order_with_products_is_not_handed_over_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 2);
        var order = await scenario.Student.PlaceStudentAppOrderAsync(StudentAppShopRequests.ProductLine(product.Variants[0].Id, 1));
        await fixture.ChangePlanAsync(scenario.Instructors.Business.Business.Id, PlanCodes.Lite);

        using var response = await owner.PutDeliveredAsync(order.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        var problem = await response.ReadProblemAsync();
        problem.GetProperty("code").GetString().ShouldBe(FeatureErrorCodes.NotInPlan);
        problem.GetProperty(FeatureErrorCodes.FeatureDetail).GetString().ShouldBe(Features.Shop);
    }
}
