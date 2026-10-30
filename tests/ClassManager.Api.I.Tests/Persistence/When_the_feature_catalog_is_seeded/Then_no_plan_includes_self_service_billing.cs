using ClassManager.Core.Domain.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_the_feature_catalog_is_seeded;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_no_plan_includes_self_service_billing(ApiFixture fixture)
{
    [Fact]
    public async Task Then_no_plan_includes_self_service_billing_Run()
    {
        await using var context = fixture.CreateDbContext(0L);

        var plansWithIt = await context.PlanFeatures.Where(planFeature => planFeature.FeatureCode == Features.SelfServiceBilling).ToListAsync();

        plansWithIt.ShouldBeEmpty();
    }
}
