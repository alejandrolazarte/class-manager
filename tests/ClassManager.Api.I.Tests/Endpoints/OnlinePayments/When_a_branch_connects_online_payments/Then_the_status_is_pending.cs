using ClassManager.Core.Domain.OnlinePayments;

namespace ClassManager.Api.I.Tests.Endpoints.OnlinePayments.When_a_branch_connects_online_payments;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_status_is_pending(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_status_is_pending_Run()
    {
        var brand = await fixture.SignUpBrandAsync(withSelfServiceBilling: false);

        await brand.HttpClient.StartOnlinePaymentOnboardingAsync();

        (await brand.HttpClient.ReadOnlinePaymentsAsync()).Status.ShouldBe(OnlinePaymentStatus.Pending);
    }
}
