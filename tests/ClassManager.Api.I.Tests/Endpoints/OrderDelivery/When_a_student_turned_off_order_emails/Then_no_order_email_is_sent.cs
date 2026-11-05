using ClassManager.Core.Domain.Notifications;
using ClassManager.Core.Emails;

namespace ClassManager.Api.I.Tests.Endpoints.OrderDelivery.When_a_student_turned_off_order_emails;

[Collection(SqlServerCollectionDefinition.Name)]
public sealed class Then_no_order_email_is_sent(ApiFixture fixture)
{
    [Fact]
    public async Task Then_no_order_email_is_sent_Run()
    {
        var scenario = await fixture.SeedStudentAppScenarioAsync();
        await scenario.Student.TurnOffAsync(NotificationCategory.Orders, NotificationChannel.Email);
        var owner = scenario.Instructors.Business.HttpClient;
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 2);
        (await scenario.Student.PostStudentAppOrderForClassAsync(
            scenario.Instructors.InstructorClassGroup.Id, StudentAppShopRequests.ProductLine(product.Variants[0].Id, 1))).EnsureSuccessStatusCode();
        var orderId = (await scenario.Student.ListStudentAppOrdersAsync()).Single().Id;

        (await owner.PostOrderPaymentAsync(orderId)).EnsureSuccessStatusCode();
        (await owner.PutOrderReadyAsync(orderId)).EnsureSuccessStatusCode();

        await fixture.WaitUntilNoTaskToAsync(scenario.Email);
        fixture.ApiFactory.EmailTransport.SentTo(scenario.Email).ShouldNotContain(email => email.Subject == ReadySubject);
    }

    private static string ReadySubject => EmailTexts.Spanish.OrderReady(new OrderEmailDetails(1, string.Empty, [], string.Empty), string.Empty, string.Empty).Subject;
}
