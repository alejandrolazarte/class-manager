using ClassManager.Core.UseCases.ImportExport.Students;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_StudentImportModule_plans_an_invalid_phone;

public sealed class Then_row_is_an_error_on_the_phone
{
    [Fact]
    public async Task Then_row_is_an_error_on_the_phone_Run()
    {
        var builder = new StudentImportBuilder();

        var plan = await builder.PlanAndAddAsync("Ana Pérez;11 55ab;;;\n");

        plan.Rows.Single().Errors.Single().Key.ShouldBe(StudentImportModule.PhoneKey);
        builder.AddedClients.ShouldBeEmpty();
    }
}
