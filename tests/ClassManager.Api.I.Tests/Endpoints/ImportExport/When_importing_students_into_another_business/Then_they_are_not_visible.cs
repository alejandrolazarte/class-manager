using ClassManager.Core.UseCases.ImportExport.Students;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.ImportExport.When_importing_students_into_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_are_not_visible(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_are_not_visible_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        await business.HttpClient.RegisterClientAsync();

        var report = await otherBusiness.HttpClient.ImportFileAsync(StudentImportModule.ModuleName, $"Alumno;Teléfono\nLucía Pérez;{ApiRequests.ClientPhoneNumber}\n");

        report.Summary.Valid.ShouldBe(1);
        var students = await business.HttpClient.GetFromJsonAsync<List<StudentSummaryResponse>>(new Uri(ApiRoutes.Students, UriKind.Relative), ApiRequests.JsonOptions);
        students.ShouldBeEmpty();
    }
}
