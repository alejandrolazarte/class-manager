using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_the_plan_does_not_include_the_shop;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_creating_a_product_is_refused(ApiFixture fixture)
{
    [Fact]
    public async Task Then_creating_a_product_is_refused_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Lite);

        using var response = await business.HttpClient.PostProductAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        var problem = await response.ReadProblemAsync();
        problem.GetProperty("code").GetString().ShouldBe(FeatureErrorCodes.NotInPlan);
        problem.GetProperty(FeatureErrorCodes.FeatureDetail).GetString().ShouldBe(Features.Shop);
    }
}
