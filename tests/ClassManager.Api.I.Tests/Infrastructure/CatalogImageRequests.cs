using System.Net.Http.Headers;

using ClassManager.Core.Domain.Images;
using ClassManager.Core.UseCases.ClassPacks;
using ClassManager.Core.UseCases.Products;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class CatalogImageRequests
{
    public const string ImageFileName = "photo.png";
    public const string PngContentType = "image/png";

    public static readonly byte[] PngImage = BrandRequests.PngLogo;

    public static byte[] OversizedPngImage() => [.. PngImage, .. new byte[CatalogImage.MaximumSizeInBytes]];

    public static Task<HttpResponseMessage> PutProductImageAsync(this HttpClient httpClient, Guid productId, byte[] content) =>
        httpClient.PutAsync(new Uri($"{ApiRoutes.Products}/{productId}{ApiRoutes.Image}", UriKind.Relative), ImageForm(content));

    public static async Task<ProductResponse> SetProductImageAsync(this HttpClient httpClient, Guid productId, byte[]? content = null)
    {
        using var response = await httpClient.PutProductImageAsync(productId, content ?? PngImage);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        return (await response.Content.ReadFromJsonAsync<ProductResponse>(ApiRequests.JsonOptions))!;
    }

    public static Task<HttpResponseMessage> DeleteProductImageAsync(this HttpClient httpClient, Guid productId) =>
        httpClient.DeleteAsync(new Uri($"{ApiRoutes.Products}/{productId}{ApiRoutes.Image}", UriKind.Relative));

    public static Task<HttpResponseMessage> PutClassPackImageAsync(this HttpClient httpClient, Guid classPackId, byte[] content) =>
        httpClient.PutAsync(new Uri($"{ApiRoutes.ClassPacks}/{classPackId}{ApiRoutes.Image}", UriKind.Relative), ImageForm(content));

    public static async Task<ClassPackResponse> SetClassPackImageAsync(this HttpClient httpClient, Guid classPackId)
    {
        using var response = await httpClient.PutClassPackImageAsync(classPackId, PngImage);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        return (await response.Content.ReadFromJsonAsync<ClassPackResponse>(ApiRequests.JsonOptions))!;
    }

    public static Task<HttpResponseMessage> DeleteClassPackImageAsync(this HttpClient httpClient, Guid classPackId) =>
        httpClient.DeleteAsync(new Uri($"{ApiRoutes.ClassPacks}/{classPackId}{ApiRoutes.Image}", UriKind.Relative));

    private static MultipartFormDataContent ImageForm(byte[] content)
    {
        var fileContent = new ByteArrayContent(content);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(PngContentType);
        return new MultipartFormDataContent { { fileContent, ImportRequests.FileFieldName, ImageFileName } };
    }
}
