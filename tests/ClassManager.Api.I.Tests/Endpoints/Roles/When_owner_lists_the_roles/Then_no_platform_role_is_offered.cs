using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Roles.When_owner_lists_the_roles;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_no_platform_role_is_offered(ApiFixture fixture)
{
    [Fact]
    public async Task Then_no_platform_role_is_offered_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        var roles = await business.HttpClient.ListRolesAsync();

        roles.Select(role => role.SystemRole).ShouldNotContain(role => role == BusinessRole.PlatformOperator || role == BusinessRole.PlatformReader);
    }
}
