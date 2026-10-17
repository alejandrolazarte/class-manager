namespace ClassManager.Api.I.Tests.Endpoints.Members.When_owner_requests_current_member;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_their_user_in_the_branch_is_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_their_user_in_the_branch_is_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        var member = await business.HttpClient.GetCurrentMemberAsync();

        var ownerUserId = await fixture.FindUserIdByAccountAsync(business.Business.Id, business.OwnerAccountId);
        member.UserId.ShouldBe(ownerUserId);
    }
}
