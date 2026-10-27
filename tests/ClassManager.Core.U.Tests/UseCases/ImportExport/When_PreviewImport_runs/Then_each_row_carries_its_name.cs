using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Instructors;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_PreviewImport_runs;

public sealed class Then_each_row_carries_its_name
{
    [Fact]
    public async Task Then_each_row_carries_its_name_Run()
    {
        var builder = new ImportUseCaseBuilder(TestData.InstructorFullName);
        var command = new PreviewImportCommand(InstructorImportModule.ModuleName, ImportUseCaseBuilder.Csv("Profesor;Email\nMarta Ruiz;x\nLaura Gómez;\n;y@example.com\n"));

        var report = await builder.PreviewUseCase.ExecuteAsync(command, CancellationToken.None);

        report.Value!.Rows.Select(row => row.Name).ShouldBe(["Marta Ruiz", TestData.InstructorFullName, null]);
    }
}
