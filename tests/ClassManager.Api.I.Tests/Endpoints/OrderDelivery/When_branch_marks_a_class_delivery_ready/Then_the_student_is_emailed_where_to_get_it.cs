namespace ClassManager.Api.I.Tests.Endpoints.OrderDelivery.When_branch_marks_a_class_delivery_ready;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_is_emailed_where_to_get_it(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_student_is_emailed_where_to_get_it_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        var owner = scenario.Coaches.Business.HttpClient;
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 2);
        using (var placed = await scenario.Student.PostStudentAppOrderForClassAsync(
            scenario.Coaches.CoachClassGroup.Id, StudentAppShopRequests.ProductLine(product.Variants[0].Id, 1)))
        {
            placed.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        var orderId = (await scenario.Student.ListStudentAppOrdersAsync()).Single().Id;
        (await owner.PutOrderPaymentAsync(orderId)).EnsureSuccessStatusCode();

        using var response = await owner.PutOrderReadyAsync(orderId);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var readyEmail = fixture.ApiFactory.EmailTransport.SentTo(scenario.Email)[^1];
        readyEmail.Subject.ShouldBe("Tu pedido está listo");
        readyEmail.TextBody.ShouldContain(CoachScenario.CoachClassGroupName);
    }
}
