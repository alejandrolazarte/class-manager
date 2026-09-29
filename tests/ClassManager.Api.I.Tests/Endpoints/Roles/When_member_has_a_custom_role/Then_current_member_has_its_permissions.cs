using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Roles.When_member_has_a_custom_role;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_current_member_has_its_permissions(ApiFixture fixture)
{
    [Fact]
    public async Task Then_current_member_has_its_permissions_Run()
    {
        var scenario = await fixture.SeedCoachScenarioAsync();
        string[] permissions = [Permissions.Business.View, Permissions.Sessions.ViewOwn, Permissions.Payments.ViewAll, Permissions.Payments.Record];
        var role = await fixture.SeedCustomRoleAsync(scenario.Business.Business.Id, permissions);
        var member = await fixture.SeedMemberAsync(scenario.Business.Business.Id, MemberRole.Custom(role), scenario.OtherInstructorId);

        var currentMember = await member.GetCurrentMemberAsync();

        currentMember.Permissions.ShouldBe(permissions, ignoreOrder: true);
        currentMember.CustomRoleId.ShouldBe(role.Id);
    }
}
