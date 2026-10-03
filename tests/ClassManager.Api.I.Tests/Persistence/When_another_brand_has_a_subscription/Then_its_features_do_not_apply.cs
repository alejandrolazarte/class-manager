using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Access;
using ClassManager.Tenancy;
using Microsoft.Extensions.DependencyInjection;

namespace ClassManager.Api.I.Tests.Persistence.When_another_brand_has_a_subscription;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_its_features_do_not_apply(ApiFixture fixture)
{
    [Fact]
    public async Task Then_its_features_do_not_apply_Run()
    {
        var freeBusiness = await fixture.SeedBusinessAsync(PlanCodes.Free);
        await fixture.SeedBusinessAsync(PlanCodes.Enterprise);

        await using var scope = fixture.ApiFactory.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<ITenantScope>().Establish(freeBusiness.Business.Id);
        var features = await scope.ServiceProvider.GetRequiredService<IFeatureAccess>().GetCurrentAsync(CancellationToken.None);

        features.PlanCode.ShouldBe(PlanCodes.Free);
        features.Has(Features.Shop).ShouldBeFalse();
    }
}
