using ClassManager.Core.UseCases.ImportExport.Students;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_StudentImportModule_exports_a_family_without_students;

public sealed class Then_it_is_not_exported
{
    [Fact]
    public async Task Then_it_is_not_exported_Run()
    {
        var builder = new StudentImportBuilder();
        builder.AddExistingClient("María Gómez", "11 7777-8888", null, null);

        var rows = await builder.Module.ExportRowsAsync(CancellationToken.None);

        rows.Value!.ShouldBeEmpty();
    }
}
