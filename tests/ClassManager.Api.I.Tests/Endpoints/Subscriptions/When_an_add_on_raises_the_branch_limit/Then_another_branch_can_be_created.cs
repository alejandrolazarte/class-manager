using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_an_add_on_raises_the_branch_limit;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_another_branch_can_be_created(ApiFixture fixture)
{
    private const int RaisedBranchLimit = 2;

    [Fact]
    public async Task Then_another_branch_can_be_created_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Free);
        await fixture.AddFeatureAsync(business, Features.Branches, RaisedBranchLimit);

        using var response = await business.HttpClient.PostBranchAsync("DF Valencia");

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }
}
