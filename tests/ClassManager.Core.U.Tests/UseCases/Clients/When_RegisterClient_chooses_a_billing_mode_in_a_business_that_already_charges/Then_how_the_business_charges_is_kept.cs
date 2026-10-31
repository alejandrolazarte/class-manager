using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Clients.When_RegisterClient_chooses_a_billing_mode_in_a_business_that_already_charges;

public sealed class Then_how_the_business_charges_is_kept
{
    [Fact]
    public async Task Then_how_the_business_charges_is_kept_Run()
    {
        var builder = new RegisterClientUseCaseBuilder();
        builder.FeeSchedule
            .Setup(repository => repository.ListDefaultFeeChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([DefaultMonthlyFeeChange.Create(BillingMonth.Parse("2026-01").Value!, BusinessBillingMode.MonthlyFee, 25000m, TestData.Today, TestData.Now).Value!.WithId()]);
        var command = RegisterClientUseCaseBuilder.ValidCommand() with { BillingMode = BusinessBillingMode.ClassPacks };

        var response = await builder.Build().ExecuteAsync(command, CancellationToken.None);

        builder.FeeSchedule.Verify(repository => repository.Add(It.IsAny<DefaultMonthlyFeeChange>()), Times.Never);
        response.Value!.BillingPlan.Kind.ShouldBe(BillingPlanKind.BusinessFee);
    }
}
