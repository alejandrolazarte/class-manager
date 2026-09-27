using ClassManager.Core.UseCases.ImportExport;
using ClassManager.Core.UseCases.ImportExport.Instructors;
using ClassManager.Core.UseCases.Instructors;

namespace ClassManager.Api.I.Tests.Endpoints.ImportExport.When_importing_instructors_into_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_they_are_not_visible(ApiFixture fixture)
{
    [Fact]
    public async Task Then_they_are_not_visible_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();

        await otherBusiness.HttpClient.ImportFileAsync(InstructorImportModule.ModuleName, "Profesor\nMarta Ruiz\n");

        var instructors = await business.HttpClient.GetFromJsonAsync<List<InstructorResponse>>(new Uri(ApiRoutes.Instructors, UriKind.Relative), ApiRequests.JsonOptions);
        instructors.ShouldBeEmpty();
    }
}
