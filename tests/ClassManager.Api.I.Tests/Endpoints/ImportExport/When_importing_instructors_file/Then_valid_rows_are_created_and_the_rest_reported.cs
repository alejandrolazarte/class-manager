using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Instructors;
using ClassManager.Core.UseCases.Instructors;

namespace ClassManager.Api.I.Tests.Endpoints.ImportExport.When_importing_instructors_file;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_valid_rows_are_created_and_the_rest_reported(ApiFixture fixture)
{
    [Fact]
    public async Task Then_valid_rows_are_created_and_the_rest_reported_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await business.HttpClient.CreateInstructorAsync("Laura Gómez");

        var report = await business.HttpClient.ImportFileAsync(InstructorImportModule.ModuleName, "Profesor;Notas\nMarta Ruiz;x\nlaura gómez;y\n;z\n");

        report.Summary.ShouldBe(new ImportSummary(Total: 3, Valid: 1, Errors: 1, Skipped: 1));
        var instructors = await business.HttpClient.GetFromJsonAsync<List<InstructorResponse>>(new Uri(ApiRoutes.Instructors, UriKind.Relative), ApiRequests.JsonOptions);
        instructors!.Select(instructor => instructor.FullName).ShouldBe(["Laura Gómez", "Marta Ruiz"]);
    }
}
