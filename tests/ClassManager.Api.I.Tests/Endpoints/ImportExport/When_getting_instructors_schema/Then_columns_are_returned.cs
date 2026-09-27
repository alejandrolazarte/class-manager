using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Instructors;

namespace ClassManager.Api.I.Tests.Endpoints.ImportExport.When_getting_instructors_schema;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_columns_are_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_columns_are_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        var schema = await business.HttpClient.GetFromJsonAsync<ImportSchemaResponse>(
            new Uri(ImportRequests.ModuleRoute(InstructorImportModule.ModuleName, ApiRoutes.SchemaAction), UriKind.Relative), ApiRequests.JsonOptions);

        schema!.Columns.Select(column => column.Key).ShouldBe([InstructorImportModule.FullNameKey]);
    }
}
