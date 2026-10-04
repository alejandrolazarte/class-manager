using System.Net.Http.Headers;

using ClassManager.Core.Domain.Businesses;
using ClassManager.Core.UseCases.Brands;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class BrandRequests
{
    public const string LogoFileName = "logo.png";

    public static readonly byte[] PngLogo =
        Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNkYPhfDwAChwGA60e6kgAAAABJRU5ErkJggg==");

    private static string BusinessBrandPath => ApiRoutes.Business + ApiRoutes.Brand;

    private static string BusinessBrandLogoPath => ApiRoutes.Business + ApiRoutes.BrandLogo;

    public static UpdateBrandCommand DeltaBrand() => new("Club Delta", "#0076B4", "#efb062", LocksTheme: true);

    public static Task<HttpResponseMessage> PutBrandAsync(this HttpClient httpClient, UpdateBrandCommand command) =>
        httpClient.PutAsJsonAsync(BusinessBrandPath, command, ApiRequests.JsonOptions);

    public static Task<BrandResponse?> GetBrandAsync(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<BrandResponse>(new Uri(BusinessBrandPath, UriKind.Relative), ApiRequests.JsonOptions);

    public static Task<BrandResponse?> GetStudentAppBrandAsync(this HttpClient httpClient) =>
        httpClient.GetFromJsonAsync<BrandResponse>(new Uri(ApiRoutes.StudentApp + ApiRoutes.Brand, UriKind.Relative), ApiRequests.JsonOptions);

    public static Task<HttpResponseMessage> PutBrandLogoAsync(this HttpClient httpClient, byte[] content)
    {
        var fileContent = new ByteArrayContent(content);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(BrandLogo.PngContentType);
        var form = new MultipartFormDataContent { { fileContent, ImportRequests.FileFieldName, LogoFileName } };

        return httpClient.PutAsync(new Uri(BusinessBrandLogoPath, UriKind.Relative), form);
    }

    public static Task<HttpResponseMessage> DeleteBrandLogoAsync(this HttpClient httpClient) =>
        httpClient.DeleteAsync(new Uri(BusinessBrandLogoPath, UriKind.Relative));

    public static Task<HttpResponseMessage> GetBrandLogoAsync(this HttpClient httpClient) =>
        httpClient.GetAsync(new Uri(BusinessBrandLogoPath, UriKind.Relative));

    public static Task<HttpResponseMessage> GetStudentAppBrandLogoAsync(this HttpClient httpClient) =>
        httpClient.GetAsync(new Uri(ApiRoutes.StudentApp + ApiRoutes.BrandLogo, UriKind.Relative));
}
