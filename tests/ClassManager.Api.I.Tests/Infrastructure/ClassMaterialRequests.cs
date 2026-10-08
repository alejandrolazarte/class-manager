using System.Net.Http.Headers;
using ClassManager.Core.UseCases.ClassGroups;

namespace ClassManager.Api.I.Tests.Infrastructure;

public static class ClassMaterialRequests
{
    public const string PdfFileName = "guia.pdf";
    public const string PdfContentType = "application/pdf";

    public static readonly byte[] Pdf = "%PDF-1.4\n1 0 obj\n<<>>\nendobj\ntrailer\n<<>>\n%%EOF\n"u8.ToArray();

    public static string MaterialPath(Guid classGroupId) => $"{ApiRoutes.ClassGroups}/{classGroupId}{ApiRoutes.Material}";

    public static Task<HttpResponseMessage> PostClassMaterialFileAsync(this HttpClient httpClient, Guid classGroupId, byte[] content) =>
        httpClient.PostAsync(new Uri(MaterialPath(classGroupId), UriKind.Relative), PdfForm(content));

    public static async Task<ClassGroupResponse> UploadClassMaterialFileAsync(this HttpClient httpClient, Guid classGroupId)
    {
        using var response = await httpClient.PostClassMaterialFileAsync(classGroupId, Pdf);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        return (await response.Content.ReadFromJsonAsync<ClassGroupResponse>(ApiRequests.JsonOptions))!;
    }

    public static Task<HttpResponseMessage> DeleteClassMaterialFileAsync(this HttpClient httpClient, Guid classGroupId) =>
        httpClient.DeleteAsync(new Uri(MaterialPath(classGroupId), UriKind.Relative));

    private static MultipartFormDataContent PdfForm(byte[] content)
    {
        var fileContent = new ByteArrayContent(content);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(PdfContentType);
        return new MultipartFormDataContent { { fileContent, ImportRequests.FileFieldName, PdfFileName } };
    }
}
