using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.OnlinePayments.When_a_viewer_opens_online_payments;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var brand = await fixture.SignUpBrandAsync(withSelfServiceBilling: false);
        using var viewer = await fixture.SeedMemberAsync(brand.BusinessId, BusinessRole.Viewer);

        using var response = await viewer.GetOnlinePaymentsAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
