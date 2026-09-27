using ClassManager.Core.UseCases.ImportExport.Instructors;
using ClassManager.ImportExport;

namespace ClassManager.Api.I.Tests.Endpoints.ImportExport.When_importing_a_file_over_the_size_limit;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400_with_error_code(ApiFixture fixture)
{
    private const int ExtraBytes = 1024;

    [Fact]
    public async Task Then_returns_400_with_error_code_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var csvText = "Profesor\n" + new string('x', ImportLimits.DefaultMaximumFileSizeInBytes + ExtraBytes);

        using var response = await business.HttpClient.PostImportFileAsync(InstructorImportModule.ModuleName, ApiRoutes.PreviewAction, csvText);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        var problem = await response.ReadProblemAsync();
        problem.GetProperty("code").GetString().ShouldBe(ImportErrorCodes.FileTooLarge);
    }
}
