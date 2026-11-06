using ClassManager.Core.Domain.OnlinePayments;

namespace ClassManager.Api.I.Tests.Endpoints.OnlinePayments.When_a_branch_never_connected;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_status_is_not_connected(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_status_is_not_connected_Run()
    {
        var brand = await fixture.SignUpBrandWithOnlinePaymentsAsync();

        var onlinePayments = await brand.HttpClient.ReadOnlinePaymentsAsync();

        onlinePayments.Status.ShouldBe(OnlinePaymentStatus.NotConnected);
    }
}
