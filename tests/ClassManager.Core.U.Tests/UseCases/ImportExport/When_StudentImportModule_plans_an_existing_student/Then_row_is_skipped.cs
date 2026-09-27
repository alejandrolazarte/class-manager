using ClassManager.Core.Domain.Students;
using ClassManager.Core.UseCases.ImportExport;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_StudentImportModule_plans_an_existing_student;

public sealed class Then_row_is_skipped
{
    [Fact]
    public async Task Then_row_is_skipped_Run()
    {
        var builder = new StudentImportBuilder();
        builder.AddExistingClient(TestData.StudentFullName);

        var plan = await builder.PlanAndAddAsync("tomás pérez;11 2233 4455;;;\n");

        plan.Rows.Single().Status.ShouldBe(ImportRowStatus.Skipped);
        plan.Rows.Single().Errors.Single().Code.ShouldBe(StudentErrorCodes.AlreadyRegistered);
        builder.AddedStudents.ShouldBeEmpty();
    }
}
