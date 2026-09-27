using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Instructors;
using ClassManager.ImportExport;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_PreviewImport_file_misses_a_required_column;

public sealed class Then_returns_validation_with_the_file_error_code
{
    [Fact]
    public async Task Then_returns_validation_with_the_file_error_code_Run()
    {
        var builder = new ImportUseCaseBuilder();
        var command = new PreviewImportCommand(InstructorImportModule.ModuleName, ImportUseCaseBuilder.Csv("Nombre completo\nMarta Ruiz\n"));

        var report = await builder.PreviewUseCase.ExecuteAsync(command, CancellationToken.None);

        report.Error!.Kind.ShouldBe(ErrorKind.Validation);
        report.Error.Code.ShouldBe(ImportErrorCodes.MissingColumns);
    }
}
