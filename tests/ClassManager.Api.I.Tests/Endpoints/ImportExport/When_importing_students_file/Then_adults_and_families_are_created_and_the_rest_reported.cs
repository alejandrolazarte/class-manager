using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Students;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.ImportExport.When_importing_students_file;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_adults_and_families_are_created_and_the_rest_reported(ApiFixture fixture)
{
    [Fact]
    public async Task Then_adults_and_families_are_created_and_the_rest_reported_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        const string CsvText = """
            Alumno;Teléfono;Fecha de nacimiento;Responsable
            Ana Pérez;11 5555-6666;;
            Lucas Gómez;11 7777-8888;07/03/2015;María Gómez
            Sofía Gómez;1177778888;;María Gómez
            L;11 9999-0000;;
            """;

        var report = await business.HttpClient.ImportFileAsync(StudentImportModule.ModuleName, CsvText);

        report.Summary.ShouldBe(new ImportSummary(Total: 4, Valid: 3, Errors: 1, Skipped: 0));
        var students = await business.HttpClient.GetFromJsonAsync<List<StudentSummaryResponse>>(new Uri(ApiRoutes.Students, UriKind.Relative), ApiRequests.JsonOptions);
        students!.Select(student => $"{student.FullName} / {student.ClientFullName}").Order().ShouldBe(
            ["Ana Pérez / Ana Pérez", "Lucas Gómez / María Gómez", "Sofía Gómez / María Gómez"]);
    }
}
