using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Students;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_importing_more_students_than_the_plan_allows;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_preview_reports_the_room_left(ApiFixture fixture)
{
    private const int SeededStudents = 29;
    private const int RoomLeft = 1;

    [Fact]
    public async Task Then_the_preview_reports_the_room_left_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Free);
        await fixture.AddFeatureAsync(business, Features.ImportExport);
        await fixture.SeedStudentsAsync(business, SeededStudents);
        const string CsvText = """
            Alumno;Teléfono;Fecha de nacimiento;Responsable
            Ana Pérez;11 5555-6666;;
            Lucas Gómez;11 7777-8888;07/03/2015;María Gómez
            """;

        using var response = await business.HttpClient.PostImportFileAsync(StudentImportModule.ModuleName, ApiRoutes.PreviewAction, CsvText);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var report = (await response.Content.ReadFromJsonAsync<ImportReport>(ApiRequests.JsonOptions))!;
        report.PlanLimit.ShouldBe(new ImportPlanLimit(Features.Students, RoomLeft));
    }
}
