using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_RegisterClient_is_the_first_choice_of_how_the_business_charges;

public sealed class Then_clients_already_registered_keep_it
{
    [Fact]
    public async Task Then_clients_already_registered_keep_it_Run()
    {
        var builder = new RegisterClientUseCaseBuilder();
        var registeredClientId = TestIds.Next();
        builder.FeeSchedule.Setup(repository => repository.ListClientIdsWithoutPlanAsync(It.IsAny<CancellationToken>())).ReturnsAsync([registeredClientId]);
        var command = RegisterClientUseCaseBuilder.ValidCommand() with { BillingMode = BusinessBillingMode.MonthlyFee };

        await builder.Build().ExecuteAsync(command, CancellationToken.None);

        builder.FeeSchedule.Verify(
            repository => repository.Add(It.Is<ClientBillingPlanChange>(change => change.ClientId == registeredClientId && change.Kind == BillingPlanKind.BusinessFee)),
            Times.Once);
    }
}
