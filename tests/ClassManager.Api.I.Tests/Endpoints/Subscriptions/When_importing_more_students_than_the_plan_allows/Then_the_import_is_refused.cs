using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Core.UseCases.ImportExport.Students;
using ClassManager.Subscriptions.Access;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_importing_more_students_than_the_plan_allows;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_import_is_refused(ApiFixture fixture)
{
    private const int SeededStudents = 29;

    [Fact]
    public async Task Then_the_import_is_refused_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Free);
        await fixture.AddFeatureAsync(business, Features.ImportExport);
        await fixture.SeedStudentsAsync(business, SeededStudents);
        const string CsvText = """
            Alumno;Teléfono;Fecha de nacimiento;Responsable
            Ana Pérez;11 5555-6666;;
            Lucas Gómez;11 7777-8888;07/03/2015;María Gómez
            """;

        using var response = await business.HttpClient.PostImportFileAsync(StudentImportModule.ModuleName, ApiRoutes.ImportAction, CsvText);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(FeatureErrorCodes.LimitReached);
        await using var context = fixture.CreateDbContext(business.Business.Id);
        (await context.Students.CountAsync()).ShouldBe(SeededStudents);
    }
}
