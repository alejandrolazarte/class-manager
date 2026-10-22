using ClassManager.Core.UseCases.Branches;

using Microsoft.EntityFrameworkCore;

namespace ClassManager.Api.I.Tests.Endpoints.Branches.When_brand_owner_creates_a_branch_with_a_location;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_branch_keeps_it(ApiFixture fixture)
{
    private const string LocationName = "Piscina Alboraya, Valencia";
    private const string LocationMapsUrl = "https://maps.app.goo.gl/r7Tn2pQx9L";

    [Fact]
    public async Task Then_the_branch_keeps_it_Run()
    {
        var business = await fixture.SeedBusinessAsync();

        using var response = await business.HttpClient.PostAsJsonAsync(
            ApiRoutes.OrganizationBranches,
            BranchRequests.BranchCommand("DF Valencia", LocationName, LocationMapsUrl),
            ApiRequests.JsonOptions);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var branch = (await response.Content.ReadFromJsonAsync<BranchResponse>(ApiRequests.JsonOptions))!;
        await using var context = fixture.CreateDbContext(branch.BusinessId);
        var storedBranch = await context.Businesses.AsNoTracking().SingleAsync(candidate => candidate.Id == branch.BusinessId);
        storedBranch.LocationName.ShouldBe(LocationName);
        storedBranch.LocationMapsUrl.ShouldBe(LocationMapsUrl);
    }
}
