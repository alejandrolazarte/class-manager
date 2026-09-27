using ClassManager.Core.UseCases.ImportExport;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_StudentImportModule_plans_a_responsable_that_differs_from_the_existing_client;

public sealed class Then_row_is_an_error_on_the_responsable
{
    [Fact]
    public async Task Then_row_is_an_error_on_the_responsable_Run()
    {
        var builder = new StudentImportBuilder();
        builder.AddExistingClient();

        var plan = await builder.PlanAndAddAsync("Lucía Ruiz;1122334455;;;Marta Ruiz\n");

        plan.Rows.Single().Errors.Single().Code.ShouldBe(ImportUseCaseErrorCodes.ContactMismatch);
    }
}
