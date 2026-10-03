using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Core.UseCases.ImportExport.Instructors;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_the_plan_does_not_include_import_export;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_export_is_refused(ApiFixture fixture)
{
    [Fact]
    public async Task Then_export_is_refused_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Free);

        using var response = await business.HttpClient.GetAsync(
            new Uri(ImportRequests.ModuleRoute(InstructorImportModule.ModuleName, ApiRoutes.ExportAction), UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        var problem = await response.ReadProblemAsync();
        problem.GetProperty("code").GetString().ShouldBe(FeatureErrorCodes.NotInPlan);
        problem.GetProperty(FeatureErrorCodes.FeatureDetail).GetString().ShouldBe(Features.ImportExport);
    }
}
