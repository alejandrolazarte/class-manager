using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Students;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_StudentImportModule_plans_the_same_student_twice;

public sealed class Then_second_row_is_an_error
{
    [Fact]
    public async Task Then_second_row_is_an_error_Run()
    {
        var builder = new StudentImportBuilder();

        var plan = await builder.PlanAndAddAsync("Lucas Gómez;11 5555-6666;;;María Gómez\nLUCAS GÓMEZ;1155556666;;;María Gómez\n");

        plan.Rows[1].Errors.Single().Code.ShouldBe(ImportUseCaseErrorCodes.DuplicateInFile);
        builder.AddedStudents.Count.ShouldBe(1);
    }
}
