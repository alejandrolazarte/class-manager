using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_DeletePayment_recorded_by_someone_else;

public sealed class Then_returns_forbidden
{
    [Fact]
    public async Task Then_returns_forbidden_Run()
    {
        var builder = new FeeUseCaseBuilder();
        var clientId = Guid.CreateVersion7();
        var payment = Payment.Create(
            clientId, 12000m, builder.CurrentMonth, TestData.Today, PaymentMethod.Cash, null, TestData.Today, TestData.Now, Guid.CreateVersion7()).Value!;
        builder.Payments.Setup(repository => repository.GetForUpdateAsync(payment.Id, It.IsAny<CancellationToken>())).ReturnsAsync(payment);
        builder.ActAsCollector(clientId);

        var response = await builder.BuildDelete().ExecuteAsync(new DeletePaymentCommand(payment.Id), CancellationToken.None);

        response.Error!.Code.ShouldBe(MemberErrorCodes.NotYours);
        builder.Payments.Verify(repository => repository.Remove(It.IsAny<Payment>()), Times.Never);
    }
}
