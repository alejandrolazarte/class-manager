using ClassManager.Core.UseCases.Clients;
using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Students;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.ImportExport.When_importing_students_with_the_phone_of_an_existing_client;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_new_students_join_the_family_and_existing_ones_are_skipped(ApiFixture fixture)
{
    [Fact]
    public async Task Then_new_students_join_the_family_and_existing_ones_are_skipped_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var client = await business.HttpClient.RegisterClientAsync(students: [new NewStudent(ApiRequests.StudentFullName, null, null)]);

        var report = await business.HttpClient.ImportFileAsync(
            StudentImportModule.ModuleName,
            $"Alumno;Teléfono\n{ApiRequests.StudentFullName};{ApiRequests.ClientPhoneNumber}\nLucía Pérez;1122334455\n");

        report.Summary.ShouldBe(new ImportSummary(Total: 2, Valid: 1, Errors: 0, Skipped: 1));
        var details = await business.HttpClient.GetFromJsonAsync<ClientDetailsResponse>(
            new Uri($"{ApiRoutes.Clients}/{client.Id}", UriKind.Relative), ApiRequests.JsonOptions);
        details!.Students.Select(student => student.FullName).Order().ShouldBe(["Lucía Pérez", ApiRequests.StudentFullName]);
    }
}
