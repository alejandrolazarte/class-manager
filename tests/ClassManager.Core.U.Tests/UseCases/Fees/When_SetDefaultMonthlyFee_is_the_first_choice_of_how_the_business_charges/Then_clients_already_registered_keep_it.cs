using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_SetDefaultMonthlyFee_is_the_first_choice_of_how_the_business_charges;

public sealed class Then_clients_already_registered_keep_it
{
    [Fact]
    public async Task Then_clients_already_registered_keep_it_Run()
    {
        var builder = new FeeUseCaseBuilder();
        builder.DefaultFeeChanges.Clear();
        var registeredClientId = TestIds.Next();
        builder.FeeSchedule.Setup(repository => repository.ListClientIdsWithoutPlanAsync(It.IsAny<CancellationToken>())).ReturnsAsync([registeredClientId]);

        await builder.BuildSetDefaultFee().ExecuteAsync(
            new SetDefaultMonthlyFeeCommand(null, null, BusinessBillingMode.ClassPacks), CancellationToken.None);

        builder.FeeSchedule.Verify(
            repository => repository.Add(It.Is<ClientBillingPlanChange>(change =>
                change.ClientId == registeredClientId
                && change.Kind == BillingPlanKind.ClassPacks
                && change.EffectiveFrom == builder.CurrentMonth.FirstDay)),
            Times.Once);
    }
}
