using ClassManager.Core.Domain.OnlinePayments;

namespace ClassManager.Api.I.Tests.Endpoints.OnlinePayments.When_a_branch_stops_using_its_account;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_not_connected(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_not_connected_Run()
    {
        var brand = await fixture.SignUpBrandWithOnlinePaymentsAsync();
        await brand.HttpClient.StartOnlinePaymentOnboardingAsync();

        using var response = await brand.HttpClient.DeleteOnlinePaymentAccountAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await brand.HttpClient.ReadOnlinePaymentsAsync()).Status.ShouldBe(OnlinePaymentStatus.NotConnected);
    }
}
