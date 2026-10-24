using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Core.UseCases.ImportExport.Instructors;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_the_brand_is_on_the_free_plan;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_export_is_allowed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_export_is_allowed_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Free);

        using var response = await business.HttpClient.GetAsync(
            new Uri(ImportRequests.ModuleRoute(InstructorImportModule.ModuleName, ApiRoutes.ExportAction), UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
