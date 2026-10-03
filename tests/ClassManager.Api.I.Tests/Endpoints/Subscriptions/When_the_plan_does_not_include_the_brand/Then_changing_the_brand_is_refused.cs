using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_the_plan_does_not_include_the_brand;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_changing_the_brand_is_refused(ApiFixture fixture)
{
    [Fact]
    public async Task Then_changing_the_brand_is_refused_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Lite);

        using var response = await business.HttpClient.PutBrandAsync(BrandRequests.DeltaBrand());

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await response.ReadProblemAsync()).GetProperty(FeatureErrorCodes.FeatureDetail).GetString().ShouldBe(Features.Brand);
    }
}
