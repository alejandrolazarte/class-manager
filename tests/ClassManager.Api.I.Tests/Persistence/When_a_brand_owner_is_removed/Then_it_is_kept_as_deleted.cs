using ClassManager.Core.Domain.Businesses;
using ClassManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Persistence.When_a_brand_owner_is_removed;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_it_is_kept_as_deleted(ApiFixture fixture)
{
    [Fact]
    public async Task Then_it_is_kept_as_deleted_Run()
    {
        var business = await fixture.SeedBusinessAsync();
        using var viewer = await fixture.SeedMemberAsync(business.Business.Id, BusinessRole.Viewer);
        var viewerMember = (await business.HttpClient.GetTeamAsync()).Members.Single(member => member.Role == BusinessRole.Viewer);
        (await business.HttpClient.PutBrandOwnerAsync(viewerMember.Id)).EnsureSuccessStatusCode();

        (await business.HttpClient.DeleteBrandOwnerAsync(viewerMember.Id)).EnsureSuccessStatusCode();

        await using var context = fixture.CreateDbContext(business.Business.Id);
        (await context.OrganizationMembers
            .IgnoreQueryFilters([SoftDeleteModelBuilderExtensions.SoftDeleteQueryFilter])
            .SingleAsync(row => row.OrganizationId == business.Business.OrganizationId && row.UserId != business.OwnerUserId))
            .DeletedOn.ShouldBe(BusinessApiFactory.Now);
    }
}
