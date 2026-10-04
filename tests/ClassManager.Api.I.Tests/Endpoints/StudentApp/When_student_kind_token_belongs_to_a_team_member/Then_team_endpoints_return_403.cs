using ClassManager.Core.Abstractions.Security;

namespace ClassManager.Api.I.Tests.Endpoints.StudentApp.When_student_kind_token_belongs_to_a_team_member;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_team_endpoints_return_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_team_endpoints_return_403_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        using var studentKindClient = fixture.CreateClientWithToken(
            fixture.CreateAccessToken(business.Business.Id, userId: business.OwnerUserId, kind: AccountKinds.Student));

        using var teamResponse = await studentKindClient.GetAsync(new Uri(ApiRoutes.Members, UriKind.Relative));
        using var meResponse = await studentKindClient.GetAsync(new Uri(ApiRoutes.Me, UriKind.Relative));

        teamResponse.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        meResponse.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
