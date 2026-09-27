using ClassManager.Core.UseCases.ImportExport;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_InstructorImportModule_plans_a_new_name;

public sealed class Then_row_is_valid
{
    [Fact]
    public async Task Then_row_is_valid_Run()
    {
        var builder = new ImportUseCaseBuilder(TestData.InstructorFullName);

        var plan = await builder.PlanInstructorsAsync("Profesor\nMarta Ruiz\n");

        plan.Rows.Single().Status.ShouldBe(ImportRowStatus.Valid);
    }
}
