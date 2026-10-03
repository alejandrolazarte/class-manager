using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_the_plan_does_not_include_the_shop;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_products_can_still_be_listed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_products_can_still_be_listed_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Lite);

        using var response = await business.HttpClient.GetAsync(new Uri(ApiRoutes.Products, UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
