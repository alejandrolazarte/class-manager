using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_brand_owner_invites_a_platform_operator;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_role_is_not_allowed(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_role_is_not_allowed_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PostInvitationAsync(MemberRequests.UniqueInviteeEmail(), BusinessRole.PlatformOperator);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await response.ReadProblemAsync()).GetProperty("code").GetString().ShouldBe(MemberErrorCodes.RoleNotAllowed);
    }
}
