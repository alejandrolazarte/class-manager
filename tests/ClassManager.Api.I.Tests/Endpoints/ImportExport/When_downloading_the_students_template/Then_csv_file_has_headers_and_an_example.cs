using ClassManager.Core.UseCases.ImportExport.Students;

namespace ClassManager.Api.I.Tests.Endpoints.ImportExport.When_downloading_the_students_template;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_csv_file_has_headers_and_an_example(ApiFixture fixture)
{
    [Fact]
    public async Task Then_csv_file_has_headers_and_an_example_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.GetAsync(
            new Uri(ImportRequests.ModuleRoute(StudentImportModule.ModuleName, ApiRoutes.TemplateAction), UriKind.Relative));

        response.Content.Headers.ContentType!.MediaType.ShouldBe(ImportRequests.CsvMediaType);
        response.Content.Headers.ContentDisposition!.FileNameStar.ShouldBe("students-template.csv");
        var csvText = await response.Content.ReadAsStringAsync();
        csvText.ShouldStartWith("Alumno;Teléfono;Email;Fecha de nacimiento;Notas alumno;Responsable;Notas responsable\r\nLucas Gómez;");
    }
}
