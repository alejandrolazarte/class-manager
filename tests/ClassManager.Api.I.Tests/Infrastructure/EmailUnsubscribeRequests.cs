using ClassManager.Core.Emails;
using ClassManager.Core.UseCases.Notifications;
using Microsoft.AspNetCore.WebUtilities;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class EmailUnsubscribeRequests
{
    private const string TokenParameter = "token";

    public static async Task<OutgoingEmail> SendOrderReadyEmailAsync(this ApiFixture fixture, StudentAppScenario scenario)
    {
        var owner = scenario.Instructors.Business.HttpClient;
        var product = await owner.RestockAsync(await owner.CreateProductAsync(), 2);
        (await scenario.Student.PostStudentAppOrderForClassAsync(
            scenario.Instructors.InstructorClassGroup.Id, StudentAppShopRequests.ProductLine(product.Variants[0].Id, 1))).EnsureSuccessStatusCode();
        var orderId = (await scenario.Student.ListStudentAppOrdersAsync()).Single().Id;
        (await owner.PostOrderPaymentAsync(orderId)).EnsureSuccessStatusCode();
        (await owner.PutOrderReadyAsync(orderId)).EnsureSuccessStatusCode();
        var readySubject = EmailTexts.Spanish.OrderReady(new OrderEmailDetails(1, string.Empty, [], string.Empty), string.Empty, string.Empty).Subject;
        return await fixture.ApiFactory.EmailTransport.WaitForEmailToAsync(scenario.Email, email => email.Subject == readySubject);
    }

    public static string UnsubscribeTokenOf(OutgoingEmail email) =>
        QueryHelpers.ParseQuery(new Uri(email.OneClickUnsubscribeUrl!).Query)[TokenParameter].ToString();

    public static Task<HttpResponseMessage> PostUnsubscribeAsync(this HttpClient httpClient, string token) =>
        httpClient.PostAsync(new Uri($"{ApiRoutes.EmailUnsubscribe}?token={Uri.EscapeDataString(token)}", UriKind.Relative), null);

    public static Task<HttpResponseMessage> PostResubscribeAsync(this HttpClient httpClient, string token) =>
        httpClient.PostAsync(new Uri($"{ApiRoutes.EmailUnsubscribe}{ApiRoutes.EmailResubscribe}?token={Uri.EscapeDataString(token)}", UriKind.Relative), null);

    public static Task<EmailUnsubscriptionResponse?> GetUnsubscriptionAsync(this HttpClient httpClient, string token) =>
        httpClient.GetFromJsonAsync<EmailUnsubscriptionResponse>(
            new Uri($"{ApiRoutes.EmailUnsubscribe}?token={Uri.EscapeDataString(token)}", UriKind.Relative), ApiRequests.JsonOptions);
}
