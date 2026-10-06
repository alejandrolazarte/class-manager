using ClassManager.Core.Domain.Authorization;

namespace ClassManager.Api.I.Tests.Endpoints.Roles.When_a_deleted_role_name_is_used_again;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_role_is_created(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_role_is_created_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var role = await business.HttpClient.CreateRoleAsync(RoleRequests.CustomRoleName, [Permissions.Business.View]);
        (await business.HttpClient.DeleteRoleAsync(role.Id!.Value)).EnsureSuccessStatusCode();

        using var response = await business.HttpClient.PostRoleAsync(RoleRequests.CustomRoleName, [Permissions.Business.View]);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }
}
