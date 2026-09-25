using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_RecordPayment_without_month;

public sealed class Then_current_month_is_used
{
    [Fact]
    public async Task Then_current_month_is_used_Run()
    {
        var builder = new FeeUseCaseBuilder();
        var client = TestData.Client();
        builder.Clients.Setup(repository => repository.GetByIdAsync(client.Id, It.IsAny<CancellationToken>())).ReturnsAsync(client);

        var response = await builder.BuildRecord().ExecuteAsync(
            new RecordPaymentCommand(client.Id, 12000m, null, null, PaymentMethod.Transfer, null), CancellationToken.None);

        response.Value!.Month.ShouldBe(builder.CurrentMonth.ToString());
        response.Value.PaidOn.ShouldBe(TestData.Today);
        builder.Payments.Verify(repository => repository.Add(It.IsAny<Payment>()), Times.Once);
    }
}
