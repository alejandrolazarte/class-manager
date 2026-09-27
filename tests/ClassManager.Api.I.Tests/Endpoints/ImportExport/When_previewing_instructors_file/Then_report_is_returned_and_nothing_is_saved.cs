using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Instructors;
using ClassManager.Core.UseCases.Instructors;

namespace ClassManager.Api.I.Tests.Endpoints.ImportExport.When_previewing_instructors_file;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_report_is_returned_and_nothing_is_saved(ApiFixture fixture)
{
    [Fact]
    public async Task Then_report_is_returned_and_nothing_is_saved_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PostImportFileAsync(InstructorImportModule.ModuleName, ApiRoutes.PreviewAction, "Profesor\nMarta Ruiz\n");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var report = await response.Content.ReadFromJsonAsync<ImportReport>(ApiRequests.JsonOptions);
        report!.Summary.Valid.ShouldBe(1);
        var instructors = await business.HttpClient.GetFromJsonAsync<List<InstructorResponse>>(new Uri(ApiRoutes.Instructors, UriKind.Relative), ApiRequests.JsonOptions);
        instructors.ShouldBeEmpty();
    }
}
