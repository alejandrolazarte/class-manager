using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Instructors;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_InstructorImportModule_plans_an_email_already_in_use;

public sealed class Then_row_is_an_error
{
    [Fact]
    public async Task Then_row_is_an_error_Run()
    {
        var builder = new ImportUseCaseBuilder();
        builder.TakenEmails.Add("marta@example.com");

        var plan = await builder.PlanInstructorsAsync("Profesor;Email\nMarta Ruiz;Marta@example.com\n");

        var row = plan.Rows.Single();
        row.Status.ShouldBe(ImportRowStatus.Error);
        row.Errors.Single().Key.ShouldBe(InstructorImportModule.EmailKey);
        row.Errors.Single().Code.ShouldBe(UserErrorCodes.EmailTaken);
    }
}
