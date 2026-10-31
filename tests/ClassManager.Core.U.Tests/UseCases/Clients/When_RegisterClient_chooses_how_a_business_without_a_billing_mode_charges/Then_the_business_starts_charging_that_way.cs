using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_RegisterClient_chooses_how_a_business_without_a_billing_mode_charges;

public sealed class Then_the_business_starts_charging_that_way
{
    [Fact]
    public async Task Then_the_business_starts_charging_that_way_Run()
    {
        var builder = new RegisterClientUseCaseBuilder();
        var command = RegisterClientUseCaseBuilder.ValidCommand() with { BillingMode = BusinessBillingMode.ClassPacks };

        var response = await builder.Build().ExecuteAsync(command, CancellationToken.None);

        builder.FeeSchedule.Verify(
            repository => repository.Add(It.Is<DefaultMonthlyFeeChange>(change =>
                change.BillingMode == BusinessBillingMode.ClassPacks && change.Amount == null && change.EffectiveFrom == new DateOnly(2026, 9, 1))),
            Times.Once);
        response.Value!.BillingPlan.Kind.ShouldBe(BillingPlanKind.ClassPacks);
    }
}
