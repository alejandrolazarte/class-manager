using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Core.UseCases.ImportExport.Instructors;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_an_add_on_adds_import_export;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_export_is_allowed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_export_is_allowed_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Free);
        await fixture.AddFeatureAsync(business, Features.ImportExport);

        using var response = await business.HttpClient.GetAsync(
            new Uri(ImportRequests.ModuleRoute(InstructorImportModule.ModuleName, ApiRoutes.ExportAction), UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
