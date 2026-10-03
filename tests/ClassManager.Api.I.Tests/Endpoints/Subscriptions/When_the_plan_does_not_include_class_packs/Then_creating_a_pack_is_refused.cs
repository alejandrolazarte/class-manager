using ClassManager.Core.Domain.Subscriptions;
using ClassManager.Subscriptions.Access;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_the_plan_does_not_include_class_packs;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_creating_a_pack_is_refused(ApiFixture fixture)
{
    [Fact]
    public async Task Then_creating_a_pack_is_refused_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Free);

        using var response = await business.HttpClient.PostClassPackAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await response.ReadProblemAsync()).GetProperty(FeatureErrorCodes.FeatureDetail).GetString().ShouldBe(Features.ClassPacks);
    }
}
