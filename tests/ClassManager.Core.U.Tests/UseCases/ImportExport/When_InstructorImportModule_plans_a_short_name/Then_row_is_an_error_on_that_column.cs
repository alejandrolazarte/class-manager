using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Instructors;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_InstructorImportModule_plans_a_short_name;

public sealed class Then_row_is_an_error_on_that_column
{
    [Fact]
    public async Task Then_row_is_an_error_on_that_column_Run()
    {
        var builder = new ImportUseCaseBuilder();

        var plan = await builder.PlanInstructorsAsync("Profesor\nM\n");

        plan.Rows.Single().Status.ShouldBe(ImportRowStatus.Error);
        plan.Rows.Single().Errors.Single().Key.ShouldBe(InstructorImportModule.FullNameKey);
    }
}
