using System.Net.Http.Headers;

using ClassManager.Core.Domain.Images;
using ClassManager.Core.UseCases.ClassPacks;
using ClassManager.Core.UseCases.Images;
using ClassManager.Core.UseCases.Products;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class CatalogImageRequests
{
    public const string ImageFileName = "photo.png";
    public const string PngContentType = "image/png";

    public static readonly byte[] PngImage = BrandRequests.PngLogo;

    public static byte[] OversizedPngImage() => [.. PngImage, .. new byte[CatalogImage.MaximumSizeInBytes]];

    public static Task<HttpResponseMessage> PostProductImageAsync(this HttpClient httpClient, Guid productId, byte[] content) =>
        httpClient.PostAsync(new Uri($"{ApiRoutes.Products}/{productId}{ApiRoutes.Images}", UriKind.Relative), ImageForm(content));

    public static async Task<ProductResponse> AddProductImageAsync(this HttpClient httpClient, Guid productId)
    {
        using var response = await httpClient.PostProductImageAsync(productId, PngImage);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        return (await response.Content.ReadFromJsonAsync<ProductResponse>(ApiRequests.JsonOptions))!;
    }

    public static Task<HttpResponseMessage> DeleteProductImageAsync(this HttpClient httpClient, Guid productId, Guid documentId) =>
        httpClient.DeleteAsync(new Uri($"{ApiRoutes.Products}/{productId}{ApiRoutes.Images}/{documentId}", UriKind.Relative));

    public static Task<HttpResponseMessage> PutProductImagesOrderAsync(this HttpClient httpClient, Guid productId, IReadOnlyList<Guid> documentIds) =>
        httpClient.PutAsJsonAsync(
            $"{ApiRoutes.Products}/{productId}{ApiRoutes.ImagesOrder}", new ReorderImagesRequest(documentIds), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PostClassPackImageAsync(this HttpClient httpClient, Guid classPackId, byte[] content) =>
        httpClient.PostAsync(new Uri($"{ApiRoutes.ClassPacks}/{classPackId}{ApiRoutes.Images}", UriKind.Relative), ImageForm(content));

    public static async Task<ClassPackResponse> AddClassPackImageAsync(this HttpClient httpClient, Guid classPackId)
    {
        using var response = await httpClient.PostClassPackImageAsync(classPackId, PngImage);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        return (await response.Content.ReadFromJsonAsync<ClassPackResponse>(ApiRequests.JsonOptions))!;
    }

    public static Task<HttpResponseMessage> DeleteClassPackImageAsync(this HttpClient httpClient, Guid classPackId, Guid documentId) =>
        httpClient.DeleteAsync(new Uri($"{ApiRoutes.ClassPacks}/{classPackId}{ApiRoutes.Images}/{documentId}", UriKind.Relative));

    private static MultipartFormDataContent ImageForm(byte[] content)
    {
        var fileContent = new ByteArrayContent(content);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(PngContentType);
        return new MultipartFormDataContent { { fileContent, ImportRequests.FileFieldName, ImageFileName } };
    }
}
