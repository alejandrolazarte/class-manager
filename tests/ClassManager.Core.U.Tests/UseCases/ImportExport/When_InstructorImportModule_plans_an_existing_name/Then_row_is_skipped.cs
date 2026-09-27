using ClassManager.Core.Domain.Instructors;
using ClassManager.Core.UseCases.ImportExport;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_InstructorImportModule_plans_an_existing_name;

public sealed class Then_row_is_skipped
{
    [Fact]
    public async Task Then_row_is_skipped_Run()
    {
        var builder = new ImportUseCaseBuilder(TestData.InstructorFullName);

        var plan = await builder.PlanInstructorsAsync("Profesor\n  LAURA GÓMEZ \n");

        plan.Rows.Single().Status.ShouldBe(ImportRowStatus.Skipped);
        plan.Rows.Single().Errors.Single().Code.ShouldBe(InstructorErrorCodes.NameTaken);
    }
}
