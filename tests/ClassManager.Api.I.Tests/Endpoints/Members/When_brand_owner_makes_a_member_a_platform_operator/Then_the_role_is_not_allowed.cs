using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_brand_owner_makes_a_member_a_platform_operator;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_role_is_not_allowed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_role_is_not_allowed_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        using var viewer = await fixture.SeedMemberAsync(business.Business.Id, BusinessRole.Viewer);
        var team = await business.HttpClient.GetTeamAsync();
        var viewerMember = team.Members.Single(member => member.Role == BusinessRole.Viewer);

        using var response = await business.HttpClient.PutMemberAsync(viewerMember.Id, BusinessRole.PlatformOperator);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(MemberErrorCodes.RoleNotAllowed);
    }
}
