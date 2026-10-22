using ClassManager.Core.Domain.Accounts;
using ClassManager.Core.Emails;

namespace ClassManager.Api.I.Tests.Endpoints.OrderDelivery.When_an_english_student_order_is_ready;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_the_student_is_emailed_in_english(ApiFixture fixture)
{
    [Fact]
    public async Task Then_the_student_is_emailed_in_english_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        (await scenario.Student.PutMyLanguageAsync(AccountLanguage.English)).EnsureSuccessStatusCode();
        var owner = scenario.Instructors.Business.HttpClient;
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 2);
        using (var placed = await scenario.Student.PostStudentAppOrderForClassAsync(
            scenario.Instructors.InstructorClassGroup.Id, StudentAppShopRequests.ProductLine(product.Variants[0].Id, 1)))
        {
            placed.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        var orderId = (await scenario.Student.ListStudentAppOrdersAsync()).Single().Id;
        (await owner.PostOrderPaymentAsync(orderId)).EnsureSuccessStatusCode();

        using var response = await owner.PutOrderReadyAsync(orderId);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var readyEmail = await fixture.ApiFactory.EmailTransport.WaitForEmailToAsync(
            scenario.Email, email => email.Subject == EmailTexts.English.OrderReady(OrderDetails, string.Empty, string.Empty).Subject);
        readyEmail.TextBody.ShouldContain(EmailTexts.English.OrderInClass(InstructorScenario.InstructorClassGroupName));
    }

    private static OrderEmailDetails OrderDetails => new(1, string.Empty, [], string.Empty);
}
