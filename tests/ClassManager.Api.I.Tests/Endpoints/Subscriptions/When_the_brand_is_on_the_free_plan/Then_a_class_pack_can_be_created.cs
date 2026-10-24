using ClassManager.Core.Domain.Subscriptions;

namespace ClassManager.Api.I.Tests.Endpoints.Subscriptions.When_the_brand_is_on_the_free_plan;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_a_class_pack_can_be_created(ApiFixture fixture)
{
    [Fact]
    public async Task Then_a_class_pack_can_be_created_Run()
    {
        var business = await fixture.SeedBusinessAsync(PlanCodes.Free);

        using var response = await business.HttpClient.PostClassPackAsync();

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }
}
