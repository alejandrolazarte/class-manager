using ClassManager.Core.UseCases.ImportExport.Instructors;

namespace ClassManager.Api.I.Tests.Endpoints.ImportExport.When_importing_as_a_user_without_access;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        using var client = fixture.CreateClientWithToken(fixture.CreateAccessToken(business.Business.Id));

        using var response = await client.PostImportFileAsync(InstructorImportModule.ModuleName, ApiRoutes.ImportAction, "Profesor\nMarta Ruiz\n");

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
