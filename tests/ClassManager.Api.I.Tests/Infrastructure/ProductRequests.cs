using ClassManager.Core.Domain.Products;
using ClassManager.Core.UseCases.Products;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class ProductRequests
{
    public const string ProductName = "Gorro de natación";
    public const decimal ProductPrice = 12m;

    public static Task<HttpResponseMessage> PostProductAsync(
        this HttpClient httpClient,
        string name = ProductName,
        StockMode stockMode = StockMode.Tracked,
        params string[] variantNames) =>
        httpClient.PostAsJsonAsync(
            ApiRoutes.Products,
            new CreateProductCommand(name, null, ProductPrice, stockMode, true, [.. variantNames.Select(variantName => new VariantChange(null, variantName))]),
            ApiRequests.JsonOptions);

    public static async Task<ProductResponse> CreateProductAsync(
        this HttpClient httpClient, StockMode stockMode = StockMode.Tracked, params string[] variantNames)
    {
        using var response = await httpClient.PostProductAsync(ProductName, stockMode, variantNames);
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<ProductResponse>(ApiRequests.JsonOptions))!;
    }

    public static Task<HttpResponseMessage> PostStockMovementAsync(
        this HttpClient httpClient, Guid productId, Guid variantId, StockMovementKind kind, int quantity, string? note = null) =>
        httpClient.PostAsJsonAsync(
            $"{ApiRoutes.Products}/{productId}{ApiRoutes.Stock}",
            new RecordStockMovementRequest(variantId, kind, quantity, note),
            ApiRequests.JsonOptions);

    public static async Task<ProductResponse> RestockAsync(this HttpClient httpClient, ProductResponse product, int quantity)
    {
        using var response = await httpClient.PostStockMovementAsync(product.Id, product.Variants[0].Id, StockMovementKind.Restock, quantity);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        return (await response.Content.ReadFromJsonAsync<ProductResponse>(ApiRequests.JsonOptions))!;
    }

    public static async Task<List<ProductResponse>> ListProductsAsync(this HttpClient httpClient) =>
        (await httpClient.GetFromJsonAsync<List<ProductResponse>>(new Uri(ApiRoutes.Products, UriKind.Relative), ApiRequests.JsonOptions))!;

    public static async Task<List<StockMovementResponse>> ListStockMovementsAsync(this HttpClient httpClient, Guid productId) =>
        (await httpClient.GetFromJsonAsync<List<StockMovementResponse>>(
            new Uri($"{ApiRoutes.Products}/{productId}{ApiRoutes.StockMovements}", UriKind.Relative), ApiRequests.JsonOptions))!;
}
