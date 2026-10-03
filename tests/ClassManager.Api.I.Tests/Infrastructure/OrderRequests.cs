using ClassManager.Core.Domain.Fees;
using ClassManager.Core.UseCases.Orders;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class OrderRequests
{
    public static Task<HttpResponseMessage> PostCounterSaleAsync(
        this HttpClient httpClient, Guid? clientId, IReadOnlyList<CounterSaleLine> lines, bool isDelivered = true) =>
        httpClient.PostAsJsonAsync(
            ApiRoutes.Orders,
            new CreateCounterSaleCommand(clientId, lines, PaymentMethod.Cash, null, null, isDelivered),
            ApiRequests.JsonOptions);

    public static async Task<OrderResponse> SellAtCounterAsync(
        this HttpClient httpClient, Guid? clientId, IReadOnlyList<CounterSaleLine> lines, bool isDelivered = true)
    {
        using var response = await httpClient.PostCounterSaleAsync(clientId, lines, isDelivered);
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<OrderResponse>(ApiRequests.JsonOptions))!;
    }

    public static CounterSaleLine PackLine(Guid classPackId) => new(classPackId, null, null, null);

    public static CounterSaleLine ProductLine(Guid variantId, int quantity) => new(null, variantId, quantity, null);

    public static async Task<List<OrderResponse>> ListOrdersAsync(this HttpClient httpClient, bool awaitingPickup = false) =>
        (await httpClient.GetFromJsonAsync<List<OrderResponse>>(
            new Uri($"{ApiRoutes.Orders}?awaitingPickup={awaitingPickup}", UriKind.Relative), ApiRequests.JsonOptions))!;

    public static async Task<OrderSummaryResponse> GetOrderSummaryAsync(this HttpClient httpClient, string month = FeeRequests.CurrentMonth) =>
        (await httpClient.GetFromJsonAsync<OrderSummaryResponse>(
            new Uri($"{ApiRoutes.Orders}{ApiRoutes.OrderSummary}?month={month}", UriKind.Relative), ApiRequests.JsonOptions))!;

    public static Task<HttpResponseMessage> PutDeliveredAsync(this HttpClient httpClient, Guid orderId) =>
        httpClient.PutAsync(new Uri($"{ApiRoutes.Orders}/{orderId}{ApiRoutes.Delivered}", UriKind.Relative), null);

    public static Task<HttpResponseMessage> PostRefundAsync(this HttpClient httpClient, Guid orderId, params RefundLine[] lines) =>
        httpClient.PostAsJsonAsync(
            $"{ApiRoutes.Orders}/{orderId}{ApiRoutes.Refunds}", new RefundOrderRequest(lines), ApiRequests.JsonOptions);
}
