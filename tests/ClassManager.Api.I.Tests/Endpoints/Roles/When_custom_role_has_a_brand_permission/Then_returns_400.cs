using ClassManager.Core.Domain.Authorization;

namespace ClassManager.Api.I.Tests.Endpoints.Roles.When_custom_role_has_a_brand_permission;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PostRoleAsync(
            RoleRequests.CustomRoleName, [Permissions.Business.View, Permissions.Brand.CreateBranches]);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
