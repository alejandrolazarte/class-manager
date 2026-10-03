using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_the_team_is_full;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_inviting_is_refused(ApiFixture fixture)
{
    [Fact]
    public async Task Then_inviting_is_refused_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Free);

        using var response = await business.HttpClient.PostInvitationAsync("coach@example.com", BusinessRole.Viewer);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await response.ReadProblemAsync()).GetProperty(FeatureErrorCodes.FeatureDetail).GetString().ShouldBe(Features.Team);
    }
}
