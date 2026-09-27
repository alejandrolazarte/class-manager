using ClassManager.Core.UseCases.ImportExport.Students;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_StudentImportModule_plans_a_future_birth_date;

public sealed class Then_row_is_an_error_on_the_birth_date
{
    [Fact]
    public async Task Then_row_is_an_error_on_the_birth_date_Run()
    {
        var builder = new StudentImportBuilder();

        var plan = await builder.PlanAndAddAsync("Ana Pérez;11 5555-6666;;01/01/2030;\n");

        plan.Rows.Single().Errors.Single().Key.ShouldBe(StudentImportModule.BirthDateKey);
    }
}
