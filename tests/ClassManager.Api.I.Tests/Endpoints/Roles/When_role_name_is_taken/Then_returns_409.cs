using ClassManager.Core.Domain.Authorization;

namespace ClassManager.Api.I.Tests.Endpoints.Roles.When_role_name_is_taken;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        await business.HttpClient.CreateRoleAsync(RoleRequests.CustomRoleName, [Permissions.Business.View]);

        using var response = await business.HttpClient.PostRoleAsync(RoleRequests.CustomRoleName, [Permissions.Members.View]);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
