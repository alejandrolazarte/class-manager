using ClassManager.Core.UseCases.ImportExport.Instructors;

namespace ClassManager.Api.I.Tests.Endpoints.ImportExport.When_exporting_instructors_of_one_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_file_has_only_that_business_instructors(ApiFixture fixture)
{
    [Fact]
    public async Task Then_file_has_only_that_business_instructors_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        await business.HttpClient.CreateInstructorAsync("Marta Ruiz");
        await otherBusiness.HttpClient.CreateInstructorAsync("Pedro Sanz");

        var exported = await business.HttpClient.DownloadWorkbookAsync(InstructorImportModule.ModuleName, ApiRoutes.ExportAction);

        ImportRequests.WorkbookRows(exported).ShouldBe([["Profesor"], ["Marta Ruiz"]]);
    }
}
