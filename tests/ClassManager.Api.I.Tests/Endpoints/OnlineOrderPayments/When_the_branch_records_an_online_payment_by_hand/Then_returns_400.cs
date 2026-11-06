using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Orders;

namespace ClassManager.Api.I.Tests.Endpoints.OnlineOrderPayments.When_the_branch_records_an_online_payment_by_hand;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        using var scenario = await fixture.SeedOnlineOrderScenarioAsync();

        using var response = await scenario.Owner.PostAsJsonAsync(
            $"{ApiRoutes.Orders}/{scenario.Order.Id}{ApiRoutes.PaymentsSegment}",
            new RecordOrderPaymentRequest(scenario.Order.Balance, PaymentMethod.Online, null),
            ApiRequests.JsonOptions);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
