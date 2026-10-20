using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Instructors;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_Export_instructors;

public sealed class Then_file_counts_its_rows
{
    [Fact]
    public async Task Then_file_counts_its_rows_Run()
    {
        var builder = new ImportUseCaseBuilder(TestData.InstructorFullName, "Marta Ruiz");

        var file = await builder.ExportUseCase.ExecuteAsync(new ExportQuery(InstructorImportModule.ModuleName), CancellationToken.None);

        file.Value!.RowCount.ShouldBe(2);
    }
}
