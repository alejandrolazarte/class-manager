using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_the_plan_does_not_include_the_brand;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_brand_colors_are_not_applied(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_brand_colors_are_not_applied_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Enterprise);
        using (var savedResponse = await business.HttpClient.PutBrandAsync(BrandRequests.DeltaBrand()))
        {
            savedResponse.EnsureSuccessStatusCode();
        }

        await fixture.ChangePlanAsync(business.Business.Id, PlanCodes.Lite);

        var brand = await business.HttpClient.GetBrandAsync();
        brand!.ThemeColor.ShouldBeNull();
        brand.AccentColor.ShouldBeNull();
        brand.LocksTheme.ShouldBeFalse();
    }
}
