using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Fees;

namespace ClassManager.Core.U.Tests.UseCases.Fees.When_SetDefaultMonthlyFee_to_class_packs;

public sealed class Then_the_business_charges_with_class_packs
{
    [Fact]
    public async Task Then_the_business_charges_with_class_packs_Run()
    {
        var builder = new FeeUseCaseBuilder();
        DefaultMonthlyFeeChange? addedChange = null;
        builder.FeeSchedule
            .Setup(repository => repository.Add(It.IsAny<DefaultMonthlyFeeChange>()))
            .Callback<DefaultMonthlyFeeChange>(change =>
            {
                addedChange = change;
                builder.DefaultFeeChanges.Add(change);
            });

        var response = await builder.BuildSetDefaultFee().ExecuteAsync(
            new SetDefaultMonthlyFeeCommand(FeeUseCaseBuilder.DefaultFee, null, BusinessBillingMode.ClassPacks), CancellationToken.None);

        addedChange!.BillingMode.ShouldBe(BusinessBillingMode.ClassPacks);
        response.Value!.BillingMode.ShouldBe(BusinessBillingMode.ClassPacks);
    }
}
