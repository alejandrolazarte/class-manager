using ClassManager.Core.UseCases.ImportExport;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_Export_module_is_unknown;

public sealed class Then_returns_not_found
{
    [Fact]
    public async Task Then_returns_not_found_Run()
    {
        var builder = new ImportUseCaseBuilder();

        var file = await builder.ExportUseCase.ExecuteAsync(new ExportQuery(ImportUseCaseBuilder.UnknownModule), CancellationToken.None);

        file.Error!.Code.ShouldBe(ImportUseCaseErrorCodes.ModuleNotFound);
    }
}
