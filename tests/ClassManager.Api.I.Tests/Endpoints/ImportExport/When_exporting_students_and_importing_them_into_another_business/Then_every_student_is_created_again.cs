using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Students;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.ImportExport.When_exporting_students_and_importing_them_into_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_every_student_is_created_again(ApiFixture fixture)
{
    [Fact]
    public async Task Then_every_student_is_created_again_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        await business.HttpClient.RegisterClientAsync(students: [new NewStudent("Lucas Pérez", new DateOnly(2015, 3, 7), "Alergia; \"leve\""), new NewStudent("Sofía Pérez", null, null)]);
        await business.HttpClient.RegisterClientAsync("Marta Ruiz", "11 5555-6666", [new NewStudent("Marta Ruiz", null, null)]);

        var exported = await business.HttpClient.DownloadWorkbookAsync(StudentImportModule.ModuleName, ApiRoutes.ExportAction);
        var report = await otherBusiness.HttpClient.ImportWorkbookAsync(StudentImportModule.ModuleName, exported);

        report.Summary.ShouldBe(new ImportSummary(Total: 3, Valid: 3, Errors: 0, Skipped: 0));
        var students = await otherBusiness.HttpClient.GetFromJsonAsync<List<StudentSummaryResponse>>(new Uri(ApiRoutes.Students, UriKind.Relative), ApiRequests.JsonOptions);
        students!.Select(student => $"{student.FullName} / {student.ClientFullName} / {student.ClientPhoneNumber} / {student.BirthDate:yyyy-MM-dd} / {student.Notes}").Order().ShouldBe(
        [
            $"Lucas Pérez / {ApiRequests.ClientFullName} / {ApiRequests.NormalizedClientPhoneNumber} / 2015-03-07 / Alergia; \"leve\"",
            "Marta Ruiz / Marta Ruiz / +541155556666 /  / ",
            $"Sofía Pérez / {ApiRequests.ClientFullName} / {ApiRequests.NormalizedClientPhoneNumber} /  / ",
        ]);
    }
}
