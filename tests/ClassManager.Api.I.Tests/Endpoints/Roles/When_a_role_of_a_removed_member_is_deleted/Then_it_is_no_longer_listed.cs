using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Roles.When_a_role_of_a_removed_member_is_deleted;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_no_longer_listed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_no_longer_listed_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var role = await fixture.SeedCustomRoleAsync(business.Business.Id, [Permissions.Business.View]);
        await fixture.SeedMemberAsync(business.Business.Id, MemberRole.Custom(role));
        var member = (await business.HttpClient.GetTeamAsync()).Members.Single(listed => listed.CustomRoleId == role.Id);
        (await business.HttpClient.DeleteMemberAsync(member.Id)).EnsureSuccessStatusCode();

        using var response = await business.HttpClient.DeleteRoleAsync(role.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await business.HttpClient.ListRolesAsync()).ShouldNotContain(listed => listed.Id == role.Id);
    }
}
