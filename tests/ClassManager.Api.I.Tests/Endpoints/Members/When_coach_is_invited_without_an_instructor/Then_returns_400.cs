using ClassManager.Core.Domain.Businesses;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_coach_is_invited_without_an_instructor;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_400(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_400_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PostInvitationAsync(MemberRequests.UniqueInviteeEmail(), BusinessRole.Coach);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
