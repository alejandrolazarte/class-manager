using ClassManager.Core.UseCases.ImportExport.Students;
using ClassManager.Core.UseCases.Students;

namespace ClassManager.Api.I.Tests.Endpoints.ImportExport.When_exporting_students_whose_users_have_other_details;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_file_has_the_users_details(ApiFixture fixture)
{
    private const string ContactUserFullName = "Ana María Pérez";
    private const string ContactUserEmail = "ana.maria@example.com";
    private const string ContactUserPhoneNumber = "+541155550101";
    private const string StudentUserFullName = "Tomás Andrés Pérez";

    [Fact]
    public async Task Then_the_file_has_the_users_details_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var client = await business.HttpClient.RegisterClientAsync(students: [new NewStudent(ApiRequests.StudentFullName, null, null)]);
        await fixture.ChangeUserOfClientAsync(business.Business.Id, client.Id, ContactUserFullName, ContactUserEmail, ContactUserPhoneNumber);
        await fixture.ChangeUserOfStudentAsync(business.Business.Id, client.Students.Single().Id, StudentUserFullName);

        var exported = await business.HttpClient.DownloadWorkbookAsync(StudentImportModule.ModuleName, ApiRoutes.ExportAction);

        var row = ImportRequests.WorkbookRows(exported)[1];
        (row[0], row[1], row[2], row[5]).ShouldBe((StudentUserFullName, "+54 1155550101", ContactUserEmail, ContactUserFullName));
    }
}
