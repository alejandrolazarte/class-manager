using ClassManager.Core.UseCases.ImportExport.Students;
using ClassManager.ImportExport.Xlsx;

namespace ClassManager.Api.I.Tests.Endpoints.ImportExport.When_downloading_the_students_template;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_workbook_has_headers_and_an_example(ApiFixture fixture)
{
    [Fact]
    public async Task Then_workbook_has_headers_and_an_example_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.GetAsync(
            new Uri(ImportRequests.ModuleRoute(StudentImportModule.ModuleName, ApiRoutes.TemplateAction), UriKind.Relative));

        response.Content.Headers.ContentType!.MediaType.ShouldBe(XlsxTabularWriter.XlsxContentType);
        response.Content.Headers.ContentDisposition!.FileNameStar.ShouldBe("students-template.xlsx");
        var rows = ImportRequests.WorkbookRows(await response.Content.ReadAsByteArrayAsync());
        rows[0].ShouldBe(["Alumno", "Teléfono", "Email de contacto", "Fecha de nacimiento", "Notas alumno", "Responsable", "Notas responsable", "Email alumno"]);
        rows[1][0].ShouldBe("Lucas Gómez");
    }
}
