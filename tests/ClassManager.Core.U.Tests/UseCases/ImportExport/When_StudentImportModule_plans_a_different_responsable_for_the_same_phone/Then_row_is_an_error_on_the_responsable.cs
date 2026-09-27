using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Students;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_StudentImportModule_plans_a_different_responsable_for_the_same_phone;

public sealed class Then_row_is_an_error_on_the_responsable
{
    [Fact]
    public async Task Then_row_is_an_error_on_the_responsable_Run()
    {
        var builder = new StudentImportBuilder();

        var plan = await builder.PlanAndAddAsync("Lucas Gómez;11 5555-6666;;;María Gómez\nSofía Ruiz;1155556666;;;Marta Ruiz\n");

        plan.Rows[1].Errors.Single().ShouldSatisfyAllConditions(
            error => error.Key.ShouldBe(StudentImportModule.ContactNameKey),
            error => error.Code.ShouldBe(ImportUseCaseErrorCodes.ContactMismatch));
    }
}
