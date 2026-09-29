using ClassManager.Core.Domain.Authorization;

namespace ClassManager.Api.I.Tests.Endpoints.Roles.When_member_is_given_a_role_of_another_business;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_404(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_404_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var otherBusiness = await fixture.SeedBusinessAsync();
        var otherRole = await fixture.SeedCustomRoleAsync(otherBusiness.Business.Id, [Permissions.Business.View]);

        using var response = await business.HttpClient.PostCustomRoleInvitationAsync(MemberRequests.UniqueInviteeEmail(), otherRole.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
