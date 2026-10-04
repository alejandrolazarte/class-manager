using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_RecordPayment_for_a_client_outside_the_scope;

public sealed class Then_returns_forbidden
{
    [Fact]
    public async Task Then_returns_forbidden_Run()
    {
        var builder = new FeeUseCaseBuilder();
        var client = TestData.Client();
        builder.Clients.Setup(repository => repository.GetByIdAsync(client.Id, It.IsAny<CancellationToken>())).ReturnsAsync(client);
        builder.ActAsCollector();

        var response = await builder.BuildRecord().ExecuteAsync(
            new RecordPaymentCommand(client.Id, 12000m, null, null, PaymentMethod.Cash, null), CancellationToken.None);

        response.Error!.Code.ShouldBe(MemberErrorCodes.NotYours);
        builder.Payments.Verify(repository => repository.Add(It.IsAny<Payment>()), Times.Never);
    }
}
