using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_StudentImportModule_adds_a_new_client_in_a_business_charging_with_class_packs;

public sealed class Then_the_client_is_added_paying_per_class
{
    [Fact]
    public async Task Then_the_client_is_added_paying_per_class_Run()
    {
        var builder = new StudentImportBuilder();
        builder.DefaultFeeChanges.Add(
            DefaultMonthlyFeeChange.Create(BillingMonth.Parse("2026-01").Value!, BusinessBillingMode.ClassPacks, null, TestData.Today, TestData.Now).Value!.WithId());

        await builder.PlanAndAddAsync("Lucas Gómez;11 5555-6666;;;María Gómez\nSofía Gómez;1155556666;;;María Gómez\n");

        builder.AddedPlanChanges.Single().Client.ShouldBeSameAs(builder.AddedClients.Single());
        builder.AddedPlanChanges.Single().Plan.ShouldBe(BillingPlan.ClassPacks);
    }
}
