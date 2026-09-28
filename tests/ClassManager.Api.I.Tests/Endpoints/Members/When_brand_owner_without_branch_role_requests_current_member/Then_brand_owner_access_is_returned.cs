using ClassManager.Core.Domain.Authorization;
using ClassManager.Core.Domain.Organizations;

namespace ClassManager.Api.I.Tests.Endpoints.Members.When_brand_owner_without_branch_role_requests_current_member;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_brand_owner_access_is_returned(ApiFixture fixture)
{
    [Fact]
    public async Task Then_brand_owner_access_is_returned_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        var brandOwnerUserId = Guid.CreateVersion7();
        await using (var context = fixture.CreateDbContext(business.Business.Id))
        {
            context.OrganizationMembers.Add(OrganizationMember.CreateBrandOwner(business.Business.OrganizationId, brandOwnerUserId));
            await context.SaveChangesAsync();
        }

        using var brandOwnerClient = fixture.CreateClientFor(business.Business.Id, brandOwnerUserId);

        var member = await brandOwnerClient.GetCurrentMemberAsync();

        member.Permissions.ShouldBe(Permissions.All, ignoreOrder: true);
        member.IsBrandOwner.ShouldBeTrue();
        member.BranchRole.ShouldBeNull();
    }
}
