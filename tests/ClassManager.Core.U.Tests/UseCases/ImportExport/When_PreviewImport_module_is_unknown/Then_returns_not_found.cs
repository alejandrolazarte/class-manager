using ClassManager.Core.UseCases.ImportExport;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_PreviewImport_module_is_unknown;

public sealed class Then_returns_not_found
{
    [Fact]
    public async Task Then_returns_not_found_Run()
    {
        var builder = new ImportUseCaseBuilder();
        var command = new PreviewImportCommand(ImportUseCaseBuilder.UnknownModule, ImportUseCaseBuilder.Csv("Profesor\nMarta Ruiz\n"));

        var report = await builder.PreviewUseCase.ExecuteAsync(command, CancellationToken.None);

        report.Error!.Code.ShouldBe(ImportUseCaseErrorCodes.ModuleNotFound);
    }
}
