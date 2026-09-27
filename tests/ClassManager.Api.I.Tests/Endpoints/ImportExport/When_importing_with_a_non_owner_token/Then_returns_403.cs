using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.ImportExport.Instructors;

namespace ClassManager.Api.I.Tests.Endpoints.ImportExport.When_importing_with_a_non_owner_token;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    private const BusinessRole RoleWithoutImportAccess = (BusinessRole)99;

    [Fact]
    public async Task Then_returns_403_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        using var client = fixture.CreateClientWithToken(fixture.CreateAccessToken(business.Business.Id, role: RoleWithoutImportAccess));

        using var response = await client.PostImportFileAsync(InstructorImportModule.ModuleName, ApiRoutes.ImportAction, "Profesor\nMarta Ruiz\n");

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
