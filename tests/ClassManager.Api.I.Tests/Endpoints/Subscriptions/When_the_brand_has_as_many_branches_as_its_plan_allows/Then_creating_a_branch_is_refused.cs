using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_the_brand_has_as_many_branches_as_its_plan_allows;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_creating_a_branch_is_refused(ApiFixture fixture)
{
    [Fact]
    public async Task Then_creating_a_branch_is_refused_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Free);

        using var response = await business.HttpClient.PostBranchAsync("DF Valencia");

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        var problem = await response.ReadProblemAsync();
        problem.GetProperty("code").GetString().ShouldBe(FeatureErrorCodes.LimitReached);
        problem.GetProperty(FeatureErrorCodes.FeatureDetail).GetString().ShouldBe(Features.Branches);
    }
}
