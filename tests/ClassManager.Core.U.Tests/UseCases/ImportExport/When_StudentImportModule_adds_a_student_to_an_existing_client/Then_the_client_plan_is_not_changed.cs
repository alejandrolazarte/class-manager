using ClassManager.Core.Domain.Fees;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_StudentImportModule_adds_a_student_to_an_existing_client;

public sealed class Then_the_client_plan_is_not_changed
{
    [Fact]
    public async Task Then_the_client_plan_is_not_changed_Run()
    {
        var builder = new StudentImportBuilder();
        builder.DefaultFeeChanges.Add(
            DefaultMonthlyFeeChange.Create(BillingMonth.Parse("2026-01").Value!, BusinessBillingMode.ClassPacks, null, TestData.Today, TestData.Now).Value!.WithId());
        builder.AddExistingClient();

        await builder.PlanAndAddAsync("Tomás Pérez;1122334455;;;\n");

        builder.AddedStudents.Count.ShouldBe(1);
        builder.AddedPlanChanges.ShouldBeEmpty();
    }
}
