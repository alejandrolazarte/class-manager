namespace ClassManager.Api.I.Tests.Endpoints.StudentAppPush.When_an_order_is_ready;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_gets_a_push(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_student_gets_a_push_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var endpoint = await scenario.SubscribeAsync();
        var owner = scenario.Coaches.Business.HttpClient;
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 2);
        (await scenario.Student.PostStudentAppOrderForClassAsync(
            scenario.Coaches.CoachClassGroup.Id, StudentAppShopRequests.ProductLine(product.Variants[0].Id, 1))).EnsureSuccessStatusCode();
        var orderId = (await scenario.Student.ListStudentAppOrdersAsync()).Single().Id;
        (await owner.PutOrderPaymentAsync(orderId)).EnsureSuccessStatusCode();

        (await owner.PutOrderReadyAsync(orderId)).EnsureSuccessStatusCode();

        var push = await fixture.ApiFactory.PushSender.WaitForPushToAsync(endpoint);
        push.Title.ShouldBe("Tu pedido está listo");
        push.Url.ShouldBe("/student-app/orders");
    }
}
