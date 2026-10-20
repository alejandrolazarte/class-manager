using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.I.Tests.Audit.When_a_branch_goes_past_the_plan_limit;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_a_plan_limit_event_is_written(ApiFixture fixture)
{
    [Fact]
    public async Task Then_a_plan_limit_event_is_written_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Free);
        var tenantId = business.Business.Id.ToString();

        using var response = await business.HttpClient.PostBranchAsync("Sede Norte");

        var lines = await SecurityEventBlobs.WaitForAsync(fixture, line => SecurityEventBlobs.Text(line, "tenantId") == tenantId);
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        lines.Where(line => SecurityEventBlobs.Text(line, "tenantId") == tenantId)
            .Select(line => (SecurityEventBlobs.Text(line, "event"), SecurityEventBlobs.Text(line, "route")))
            .ShouldHaveSingleItem()
            .ShouldBe((nameof(SecurityEventType.PlanLimit), ApiRoutes.OrganizationBranches));
    }
}
