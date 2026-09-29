using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_RecordPayment_by_a_collector;

public sealed class Then_the_collector_is_recorded
{
    [Fact]
    public async Task Then_the_collector_is_recorded_Run()
    {
        var builder = new FeeUseCaseBuilder();
        var client = TestData.Client();
        builder.Clients.Setup(repository => repository.GetByIdAsync(client.Id, It.IsAny<CancellationToken>())).ReturnsAsync(client);
        builder.ActAsCollector(client.Id);
        Payment? addedPayment = null;
        builder.Payments.Setup(repository => repository.Add(It.IsAny<Payment>())).Callback<Payment>(payment => addedPayment = payment);

        var response = await builder.BuildRecord().ExecuteAsync(
            new RecordPaymentCommand(client.Id, 12000m, null, null, PaymentMethod.Cash, null), CancellationToken.None);

        response.IsSuccess.ShouldBeTrue();
        addedPayment!.RecordedByUserId.ShouldBe(builder.CurrentUserId);
    }
}
