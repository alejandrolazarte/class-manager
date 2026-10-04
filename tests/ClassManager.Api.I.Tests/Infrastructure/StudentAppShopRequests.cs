using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Orders;
using ClassManager.Core.UseCases.Orders;
using ClassManager.Core.UseCases.StudentApp;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class StudentAppShopRequests
{
    public static async Task<StudentAppShopResponse> GetStudentAppShopAsync(this HttpClient httpClient) =>
        (await httpClient.GetFromJsonAsync<StudentAppShopResponse>(
            new Uri(ApiRoutes.StudentApp + ApiRoutes.StudentAppShop, UriKind.Relative), ApiRequests.JsonOptions))!;

    public static Task<HttpResponseMessage> PostStudentAppOrderAsync(this HttpClient httpClient, params StudentAppOrderLine[] lines) =>
        httpClient.PostAsJsonAsync(ApiRoutes.StudentApp + ApiRoutes.StudentAppOrders, new PlaceStudentAppOrderCommand(lines), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PostStudentAppOrderForClassAsync(this HttpClient httpClient, Guid classGroupId, params StudentAppOrderLine[] lines) =>
        httpClient.PostAsJsonAsync(
            ApiRoutes.StudentApp + ApiRoutes.StudentAppOrders,
            new PlaceStudentAppOrderCommand(lines, DeliveryMethod.InClass, classGroupId),
            ApiRequests.JsonOptions);

    public static async Task<StudentAppOrderResponse> PlaceStudentAppOrderAsync(this HttpClient httpClient, params StudentAppOrderLine[] lines)
    {
        using var response = await httpClient.PostStudentAppOrderAsync(lines);
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<StudentAppOrderResponse>(ApiRequests.JsonOptions))!;
    }

    public static StudentAppOrderLine ProductLine(Guid variantId, int quantity) => new(null, variantId, quantity);

    public static StudentAppOrderLine PackLine(Guid classPackId) => new(classPackId, null, 1);

    public static async Task<List<StudentAppOrderResponse>> ListStudentAppOrdersAsync(this HttpClient httpClient) =>
        (await httpClient.GetFromJsonAsync<List<StudentAppOrderResponse>>(
            new Uri(ApiRoutes.StudentApp + ApiRoutes.StudentAppOrders, UriKind.Relative), ApiRequests.JsonOptions))!;

    public static Task<HttpResponseMessage> PutStudentAppOrderCancellationAsync(this HttpClient httpClient, Guid orderId) =>
        httpClient.PutAsync(
            new Uri($"{ApiRoutes.StudentApp}{ApiRoutes.StudentAppOrders}/{orderId}{ApiRoutes.Cancellation}", UriKind.Relative), null);

    public static Task<HttpResponseMessage> PutOrderPaymentAsync(this HttpClient httpClient, Guid orderId) =>
        httpClient.PutAsJsonAsync(
            $"{ApiRoutes.Orders}/{orderId}{ApiRoutes.Payment}",
            new ConfirmOrderPaymentRequest(PaymentMethod.Cash, null),
            ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PutOrderReadyAsync(this HttpClient httpClient, Guid orderId) =>
        httpClient.PutAsync(new Uri($"{ApiRoutes.Orders}/{orderId}{ApiRoutes.Ready}", UriKind.Relative), null);

    public static async Task<List<ClassDeliveryResponse>> ListClassDeliveriesAsync(this HttpClient httpClient, Guid classGroupId) =>
        (await httpClient.GetFromJsonAsync<List<ClassDeliveryResponse>>(
            new Uri($"{ApiRoutes.ClassGroups}/{classGroupId}{ApiRoutes.Deliveries}", UriKind.Relative), ApiRequests.JsonOptions))!;

    public static Task<HttpResponseMessage> PutClassDeliveryAsync(this HttpClient httpClient, Guid classGroupId, Guid orderId) =>
        httpClient.PutAsync(new Uri($"{ApiRoutes.ClassGroups}/{classGroupId}{ApiRoutes.Deliveries}/{orderId}", UriKind.Relative), null);

    public static Task<HttpResponseMessage> PutOrderCancellationAsync(this HttpClient httpClient, Guid orderId) =>
        httpClient.PutAsync(new Uri($"{ApiRoutes.Orders}/{orderId}{ApiRoutes.Cancellation}", UriKind.Relative), null);
}
