
namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_InstructorImportModule_adds_valid_rows;

public sealed class Then_only_valid_rows_are_added
{
    [Fact]
    public async Task Then_only_valid_rows_are_added_Run()
    {
        var builder = new ImportUseCaseBuilder(TestData.InstructorFullName);
        var plan = await builder.PlanInstructorsAsync("Profesor\nMarta Ruiz\nLaura Gómez\nM\nPedro Sanz\n");

        plan.AddValidRows();

        builder.AddedInstructors.Select(instructor => instructor.FullName).ShouldBe(["Marta Ruiz", "Pedro Sanz"]);
    }
}
