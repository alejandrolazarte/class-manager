using ClassManager.Core.Domain.Fees;
using ClassManager.Core.Domain.Orders;
using ClassManager.Core.UseCases.Families;
using ClassManager.Core.UseCases.Orders;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class FamilyShopRequests
{
    public static async Task<FamilyShopResponse> GetFamilyShopAsync(this HttpClient httpClient) =>
        (await httpClient.GetFromJsonAsync<FamilyShopResponse>(
            new Uri(ApiRoutes.Family + ApiRoutes.FamilyShop, UriKind.Relative), ApiRequests.JsonOptions))!;

    public static Task<HttpResponseMessage> PostFamilyOrderAsync(this HttpClient httpClient, params FamilyOrderLine[] lines) =>
        httpClient.PostAsJsonAsync(ApiRoutes.Family + ApiRoutes.FamilyOrders, new PlaceFamilyOrderCommand(lines), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PostFamilyOrderForClassAsync(this HttpClient httpClient, Guid classGroupId, params FamilyOrderLine[] lines) =>
        httpClient.PostAsJsonAsync(
            ApiRoutes.Family + ApiRoutes.FamilyOrders,
            new PlaceFamilyOrderCommand(lines, DeliveryMethod.InClass, classGroupId),
            ApiRequests.JsonOptions);

    public static async Task<FamilyOrderResponse> PlaceFamilyOrderAsync(this HttpClient httpClient, params FamilyOrderLine[] lines)
    {
        using var response = await httpClient.PostFamilyOrderAsync(lines);
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<FamilyOrderResponse>(ApiRequests.JsonOptions))!;
    }

    public static FamilyOrderLine ProductLine(Guid variantId, int quantity) => new(null, variantId, quantity);

    public static FamilyOrderLine PackLine(Guid classPackId) => new(classPackId, null, 1);

    public static async Task<List<FamilyOrderResponse>> ListFamilyOrdersAsync(this HttpClient httpClient) =>
        (await httpClient.GetFromJsonAsync<List<FamilyOrderResponse>>(
            new Uri(ApiRoutes.Family + ApiRoutes.FamilyOrders, UriKind.Relative), ApiRequests.JsonOptions))!;

    public static Task<HttpResponseMessage> PutFamilyOrderCancellationAsync(this HttpClient httpClient, Guid orderId) =>
        httpClient.PutAsync(
            new Uri($"{ApiRoutes.Family}{ApiRoutes.FamilyOrders}/{orderId}{ApiRoutes.Cancellation}", UriKind.Relative), null);

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
