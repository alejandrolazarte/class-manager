using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Instructors;
using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.I.Tests.Audit.When_instructors_are_exported_through_the_api;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_export_is_logged_with_its_row_count(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_export_is_logged_with_its_row_count_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await business.HttpClient.CreateInstructorAsync("Marta Ruiz");
        await business.HttpClient.CreateInstructorAsync("Pedro Sanz");

        var exported = await business.HttpClient.DownloadWorkbookAsync(InstructorImportModule.ModuleName, ApiRoutes.ExportAction);

        var row = await CommandAuditRows.WaitForAsync(fixture, business.Business.Id, nameof(ExportQuery));
        var exportedRowsWithoutHeader = ImportRequests.WorkbookRows(exported).Count - 1;
        (row.GetInt32(nameof(CommandAuditEntry.RowCount)), row.GetString(nameof(CommandAuditEntry.Payload)))
            .ShouldBe((exportedRowsWithoutHeader, $$"""{"module":"{{InstructorImportModule.ModuleName}}"}"""));
    }
}
