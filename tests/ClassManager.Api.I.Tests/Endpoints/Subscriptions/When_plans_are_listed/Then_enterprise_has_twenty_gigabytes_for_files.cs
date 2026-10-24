using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_plans_are_listed;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_enterprise_has_twenty_gigabytes_for_files(ApiFixture fixture)
{
    private const int TwentyGigabytesInMegabytes = 20 * 1024;

    [Fact]
    public async Task Then_enterprise_has_twenty_gigabytes_for_files_Run()
    {
        using var anonymousClient = fixture.ApiFactory.CreateClient();

        var plans = await anonymousClient.ListPlansAsync();

        plans.Single(plan => plan.Code == PlanCodes.Enterprise).Features
            .Single(feature => feature.Code == Features.DocumentStorage).Limit
            .ShouldBe(TwentyGigabytesInMegabytes);
    }
}
