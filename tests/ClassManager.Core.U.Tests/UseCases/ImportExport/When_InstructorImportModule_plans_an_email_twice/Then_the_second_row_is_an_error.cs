using ClassManager.Core.UseCases.ImportExport;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_InstructorImportModule_plans_an_email_twice;

public sealed class Then_the_second_row_is_an_error
{
    [Fact]
    public async Task Then_the_second_row_is_an_error_Run()
    {
        var builder = new ImportUseCaseBuilder();

        var plan = await builder.PlanInstructorsAsync("Profesor;Email\nMarta Ruiz;marta@example.com\nLaura Sosa;marta@example.com\n");

        plan.Rows.Select(row => row.Status).ShouldBe([ImportRowStatus.Valid, ImportRowStatus.Error]);
    }
}
