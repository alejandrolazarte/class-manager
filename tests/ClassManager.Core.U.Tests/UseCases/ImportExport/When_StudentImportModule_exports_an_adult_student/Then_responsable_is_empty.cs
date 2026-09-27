using ClassManager.Core.UseCases.ImportExport.Students;

namespace ClassManager.Core.U.Tests.UseCases.ImportExport.When_StudentImportModule_exports_an_adult_student;

public sealed class Then_responsable_is_empty
{
    [Fact]
    public async Task Then_responsable_is_empty_Run()
    {
        var builder = new StudentImportBuilder();
        var client = builder.AddExistingClient("Ana Pérez", "11 5555-6666", "ana@example.com", null);
        builder.AddExistingStudent(client, "Ana Pérez", null);

        var rows = await builder.Module.ExportRowsAsync(CancellationToken.None);

        rows.Value!.Single()[StudentImportModule.ContactNameKey].ShouldBeNull();
    }
}
