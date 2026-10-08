using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Endpoints.StudentAppNews.When_an_order_of_the_student_is_ready;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_news_points_to_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_news_points_to_it_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Instructors.Business.HttpClient;
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 2);
        (await scenario.Student.PostStudentAppOrderAsync(StudentAppShopRequests.ProductLine(product.Variants[0].Id, 1))).EnsureSuccessStatusCode();
        var orderId = (await scenario.Student.ListStudentAppOrdersAsync()).Single().Id;
        (await owner.PutOrderPaymentAsync(orderId)).EnsureSuccessStatusCode();
        (await owner.PutOrderReadyAsync(orderId)).EnsureSuccessStatusCode();

        var news = await scenario.Student.GetStudentAppNewsAsync();

        news!.Items.Single(item => item.Kind == StudentAppNewsKind.OrderReady).OrderId.ShouldBe(orderId);
    }
}
