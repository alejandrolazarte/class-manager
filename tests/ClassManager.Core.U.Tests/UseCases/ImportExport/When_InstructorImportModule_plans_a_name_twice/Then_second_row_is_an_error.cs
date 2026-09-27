using ClassManager.Core.UseCases.ImportExport;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_InstructorImportModule_plans_a_name_twice;

public sealed class Then_second_row_is_an_error
{
    [Fact]
    public async Task Then_second_row_is_an_error_Run()
    {
        var builder = new ImportUseCaseBuilder();

        var plan = await builder.PlanInstructorsAsync("Profesor\nMarta Ruiz\nmarta ruiz\n");

        plan.Rows.Select(row => row.Status).ShouldBe([ImportRowStatus.Valid, ImportRowStatus.Error]);
        plan.Rows[1].Errors.Single().Code.ShouldBe(ImportUseCaseErrorCodes.DuplicateInFile);
    }
}
