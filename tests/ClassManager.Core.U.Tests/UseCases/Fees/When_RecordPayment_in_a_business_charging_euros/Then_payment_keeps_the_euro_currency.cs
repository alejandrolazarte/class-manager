using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_RecordPayment_in_a_business_charging_euros;

public sealed class Then_payment_keeps_the_euro_currency
{
    [Fact]
    public async Task Then_payment_keeps_the_euro_currency_Run()
    {
        var builder = new FeeUseCaseBuilder();
        builder.Business.UpdateSettings(builder.Business.Name, TestData.Argentina(), TestData.BuenosAiresTimeZoneId, TestData.EuroCurrencyCode);
        var client = TestData.Client();
        builder.Clients.Setup(repository => repository.GetByIdAsync(client.Id, It.IsAny<CancellationToken>())).ReturnsAsync(client);

        var response = await builder.BuildRecord().ExecuteAsync(
            new RecordPaymentCommand(client.Id, 50m, null, null, PaymentMethod.Card, null), CancellationToken.None);

        response.Value!.CurrencyCode.ShouldBe(TestData.EuroCurrencyCode);
        builder.Payments.Verify(repository => repository.Add(It.Is<Payment>(payment => payment.CurrencyCode == TestData.EuroCurrencyCode)), Times.Once);
    }
}
