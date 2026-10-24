using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_plans_are_listed;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_every_plan_includes_import_export(ApiFixture fixture)
{
    [Fact]
    public async Task Then_every_plan_includes_import_export_Run()
    {
        using var anonymousClient = fixture.ApiFactory.CreateClient();

        var plans = await anonymousClient.ListPlansAsync();

        plans.ShouldAllBe(plan => plan.Features.Any(feature => feature.Code == Features.ImportExport));
    }
}
