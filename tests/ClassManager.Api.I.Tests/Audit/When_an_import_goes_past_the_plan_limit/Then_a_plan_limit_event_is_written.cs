using System.Globalization;
using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Core.UseCases.ImportExport.Students;
using ClassManager.Infrastructure.Audit;

namespace ClassManager.Api.I.Tests.Audit.When_an_import_goes_past_the_plan_limit;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_a_plan_limit_event_is_written(ApiFixture fixture)
{
    private const int SeededStudents = 29;

    [Fact]
    public async Task Then_a_plan_limit_event_is_written_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Free);
        await fixture.SeedStudentsAsync(business, SeededStudents);
        var tenantId = business.Business.Id.ToString(CultureInfo.InvariantCulture);
        const string CsvText = """
            Alumno;Teléfono;Fecha de nacimiento;Responsable
            Ana Pérez;11 5555-6666;;
            Lucas Gómez;11 7777-8888;07/03/2015;María Gómez
            """;

        using var response = await business.HttpClient.PostImportFileAsync(StudentImportModule.ModuleName, ApiRoutes.ImportAction, CsvText);

        var lines = await SecurityEventBlobs.WaitForAsync(fixture, line => SecurityEventBlobs.Text(line, "tenantId") == tenantId);
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        lines.Where(line => SecurityEventBlobs.Text(line, "tenantId") == tenantId)
            .Select(line => SecurityEventBlobs.Text(line, "event"))
            .ShouldHaveSingleItem()
            .ShouldBe(nameof(SecurityEventType.PlanLimit));
    }
}
