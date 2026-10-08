using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Roles.When_system_role_is_copied;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_custom_role_is_listed_with_its_permissions(ApiFixture fixture)
{
    [Fact]
    public async Task Then_custom_role_is_listed_with_its_permissions_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        string[] permissions = [.. SystemRolePermissions.Of(BusinessRole.Instructor), Permissions.Payments.Record];

        var created = await business.HttpClient.CreateRoleAsync(RoleRequests.CustomRoleName, permissions, nameof(BusinessRole.Instructor));

        var roles = await business.HttpClient.ListRolesAsync();
        var listed = roles.Single(role => role.Id == created.Id);
        listed.Permissions.ShouldBe(permissions, ignoreOrder: true);
        listed.CopiedFrom.ShouldBe(nameof(BusinessRole.Instructor));
    }
}
