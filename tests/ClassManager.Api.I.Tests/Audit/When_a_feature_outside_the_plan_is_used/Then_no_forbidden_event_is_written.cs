using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.I.Tests.Audit.When_a_feature_outside_the_plan_is_used;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_no_forbidden_event_is_written(ApiFixture fixture)
{
    private const string BrandRoute = ApiRoutes.Business + ApiRoutes.Brand;

    [Fact]
    public async Task Then_no_forbidden_event_is_written_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Lite);
        using var viewer = await fixture.SeedMemberAsync(business.Business.Id, BusinessRole.Viewer);
        var tenantId = business.Business.Id.ToString();

        using var planLimitResponse = await business.HttpClient.PutBrandAsync(BrandRequests.DeltaBrand());
        using var forbiddenResponse = await viewer.PostClientAsync(ApiRequests.ClientFullName, ApiRequests.ClientPhoneNumber, students: null);

        var lines = await SecurityEventBlobs.WaitForAsync(fixture, line =>
            SecurityEventBlobs.Text(line, "event") == nameof(SecurityEventType.Forbidden) && SecurityEventBlobs.Text(line, "tenantId") == tenantId);
        (planLimitResponse.StatusCode, forbiddenResponse.StatusCode).ShouldBe((HttpStatusCode.Forbidden, HttpStatusCode.Forbidden));
        lines.ShouldNotContain(line => SecurityEventBlobs.Text(line, "tenantId") == tenantId && SecurityEventBlobs.Text(line, "route") == BrandRoute);
    }
}
