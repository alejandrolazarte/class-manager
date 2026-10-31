using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.Domain.Fees.When_new_client_joins_a_business_that_has_not_chosen_how_it_charges;

public sealed class Then_no_plan_is_recorded
{
    [Fact]
    public void Then_no_plan_is_recorded_Run()
    {
        var planChanges = ClientBillingPlanChange.ForNewClient(TestData.Client(), [], TestData.Today, TestData.Now);

        planChanges.ShouldBeEmpty();
    }
}
