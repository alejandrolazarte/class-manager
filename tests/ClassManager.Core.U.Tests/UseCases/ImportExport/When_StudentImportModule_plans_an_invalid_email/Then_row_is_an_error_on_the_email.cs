using ClassManager.Core.UseCases.ImportExport.Students;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_StudentImportModule_plans_an_invalid_email;

public sealed class Then_row_is_an_error_on_the_email
{
    [Fact]
    public async Task Then_row_is_an_error_on_the_email_Run()
    {
        var builder = new StudentImportBuilder();

        var plan = await builder.PlanAndAddAsync("Ana Pérez;11 5555-6666;ana-at-example.com;;\n");

        plan.Rows.Single().Errors.Single().Key.ShouldBe(StudentImportModule.EmailKey);
    }
}
