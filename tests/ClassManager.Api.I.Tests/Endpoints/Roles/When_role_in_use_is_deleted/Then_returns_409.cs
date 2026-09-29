using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Roles.When_role_in_use_is_deleted;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_409(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_409_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var role = await fixture.SeedCustomRoleAsync(business.Business.Id, [Permissions.Business.View]);
        await fixture.SeedMemberAsync(business.Business.Id, MemberRole.Custom(role));

        using var response = await business.HttpClient.DeleteRoleAsync(role.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}
