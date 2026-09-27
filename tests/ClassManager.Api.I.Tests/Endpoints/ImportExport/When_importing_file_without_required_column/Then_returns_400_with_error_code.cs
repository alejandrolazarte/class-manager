using ClassManager.Core.UseCases.ImportExport.Instructors;
using ClassManager.ImportExport;

namespace ClassManager.Api.I.Tests.Endpoints.ImportExport.When_importing_file_without_required_column;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400_with_error_code(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_with_error_code_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PostImportFileAsync(InstructorImportModule.ModuleName, ApiRoutes.ImportAction, "Nombre completo\nMarta Ruiz\n");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var problem = await response.ReadProblemAsync();
        problem.GetProperty("code").GetString().ShouldBe(ImportErrorCodes.MissingColumns);
    }
}
