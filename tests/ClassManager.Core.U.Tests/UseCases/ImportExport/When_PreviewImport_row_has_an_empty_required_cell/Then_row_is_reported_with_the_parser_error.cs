using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Instructors;
using ClassManager.ImportExport;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_PreviewImport_row_has_an_empty_required_cell;

public sealed class Then_row_is_reported_with_the_parser_error
{
    [Fact]
    public async Task Then_row_is_reported_with_the_parser_error_Run()
    {
        var builder = new ImportUseCaseBuilder();
        var command = new PreviewImportCommand(InstructorImportModule.ModuleName, ImportUseCaseBuilder.Csv("Profesor;Notas\n;sin nombre\n"));

        var report = await builder.PreviewUseCase.ExecuteAsync(command, CancellationToken.None);

        var row = report.Value!.Rows.Single();
        row.Line.ShouldBe(2);
        row.Errors.Single().Code.ShouldBe(ImportErrorCodes.Required);
    }
}
