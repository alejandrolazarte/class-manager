namespace ClassManager.Api.I.Tests.Endpoints.OrderDelivery.When_coach_hands_over_in_a_class_that_is_not_theirs;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_returns_403(ApiFixture fixture)
{
    [Fact]
    public async Task Then_returns_403_Run()
    {
        var scenario = await fixture.SeedFamilyScenarioAsync();
        var otherFamily = await fixture.InviteFamilyOfAsync(scenario.Coaches, CoachScenario.OtherStudentFullName);
        var owner = scenario.Coaches.Business.HttpClient;
        var otherClassGroupId = scenario.Coaches.OtherClassGroup.Id;
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 2);
        using (var placed = await otherFamily.Family.PostFamilyOrderForClassAsync(
            otherClassGroupId, FamilyShopRequests.ProductLine(product.Variants[0].Id, 1)))
        {
            placed.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        var orderId = (await otherFamily.Family.ListFamilyOrdersAsync()).Single().Id;
        (await owner.PutOrderPaymentAsync(orderId)).EnsureSuccessStatusCode();

        using var response = await scenario.Coaches.Coach.PutClassDeliveryAsync(otherClassGroupId, orderId);

        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
